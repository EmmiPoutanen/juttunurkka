
/*
Copyright 2021 Emma Kemppainen, Jesse Huttunen, Tanja Kultala, Niklas Arjasmaa

This file is part of "Mieliala kysely".

Mieliala kysely is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, version 3 of the License.

Mieliala kysely is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Mieliala kysely.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Prototype
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LuoKyselyToimenpiteet : ContentPage
    {

        public IList<CollectionItem> Items { get; set; }
        public List<string> tempActivities = new List<string>();

        public class CollectionItem

        {
            public Emoji Emoji { get; set; }
            public IList<string> ActivityChoises { get; set; }
            public ObservableCollection<object> Selected { get; set; }
            public CollectionItem(Emoji emoji, IList<string> activities)
            {
                Emoji = emoji;
                ActivityChoises = activities;

                Selected = new ObservableCollection<object>();
                foreach (var item in emoji.activities)
                {
                    Selected.Add(ActivityChoises[ActivityChoises.IndexOf(item)]);
                }
            }
        }

        public LuoKyselyToimenpiteet()
        {
            InitializeComponent();

            //alustus
            Items = new List<CollectionItem>();
            foreach (var item in SurveyManager.GetInstance().GetSurvey().emojis)
            {
                Items.Add(new CollectionItem(item, Const.activities[item.ID]));
            }

            BindingContext = this;
        }

        async void EdellinenButtonClicked(object sender, EventArgs e)
        {
            var res = await DisplayAlert("Tahdotko varmasti keskeyttää kyselynluonnin?", "", "Kyllä", "Ei");

            if (res == true)
            {
                //survey resetoidaan
                SurveyManager.GetInstance().ResetSurvey();

                //Jos ollaan edit tilassa, niin siirrytään takaisin kyselyntarkastelu sivulle, muutoin main menuun
                if (Main.GetInstance().GetMainState() == Main.MainState.Editing)
                {
                    Main.GetInstance().BrowseSurveys();
                    await Navigation.PopAsync();
                }
                else
                {
                    // siirrytään etusivulle
                    await Navigation.PopToRootAsync();
                }

            }
            else return;
        }

        /*
        private void btnPopupButton_Clicked(object sender, EventArgs e)
        {


            if (popupSelection.IsVisible == false)
            {

                popupSelection.IsVisible = true;
            }

            else if (popupSelection.IsVisible == true)
            {

                popupSelection.IsVisible = false;
            }

        }*/

        void ButtonClicked(object sender, EventArgs e)
        {

            if (sender is Button b && b.Parent is Grid g)
            {
                //tassa harjoitusta
                 
                if (b.TextColor == Color.Black)
                {
                    b.TextColor = Color.White;
                    b.BackgroundColor = Color.Blue;
                    b.CornerRadius = 10;
                    Console.WriteLine(b.Text + " klikattu");
                    tempActivities.Add(b.Text);
                }
                else if (b.TextColor == Color.White)
                {
                    b.TextColor = Color.Black;
                    b.BackgroundColor = Color.White;
                    b.CornerRadius=10;
                    foreach (var word in tempActivities)
                    {
                        if (word.Equals(b.Text))
                        {
                            tempActivities.Remove(word);
                            break;
                        }
                        else
                        {
                            //just go on
                        }
                    }
                }
                else
                {

                }
            }
            else
            {

            }
            foreach (var word in tempActivities)
            {
                Console.WriteLine(word);
            }


        }


        void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is CollectionView cv && cv.SelectionChangedCommandParameter is CollectionItem item)
            {

            }
        }


        async void JatkaButtonClicked(object sender, EventArgs e)
        {
            
			//error if not all emojis have at least 1 selected activity
/*			if (!ActivitiesSet())
			{
                await DisplayAlert("Kaikkia valintoja ei ole tehty", "Sinun on valittava jokaiselle mielialalle vähintään yksi aktiviteetti", "OK");
                return;
            }
*/
            //asetetaan emojit survey olioon
            //tässä kokeilua if-else jatka tasta
            if (SurveyManager.GetInstance().GetSurvey().emojis[0].Name.Equals("Iloinen"))
            {
                Console.WriteLine("eka emoji listassa iloinen");
                SurveyManager.GetInstance().GetSurvey().emojis[0].activities=tempActivities;
            }
            else
            {

            }
            //kokonaan kommenttiin PP
     /*       List<Emoji> tempEmojis = new List<Emoji>();
            foreach (var item in Items)
            {
                //         otin pois ja siirsin ylös//PP    List<string> tempActivities = new List<string>();
                foreach (var selection in item.Selected)
                {
                    //otin pois ja siirsin ylös/PP                  tempActivities.Add(selection as string);
                }
                // otin pois ja siirsin ylös/PP              item.Emoji.activities = tempActivities;
                tempEmojis.Add(item.Emoji);
            }
            SurveyManager.GetInstance().GetSurvey().emojis = tempEmojis;*/

            // siirrytään "Luo kysely -lopetus" sivulle 
            await Navigation.PushAsync(new LuoKyselyLopetus()); ;
        }
/*
        //function which checks whether the user has selected at least 1 activity for each emoji.
        private bool ActivitiesSet()
        {
            foreach (var item in Items)
            {
                if (item.Selected.Count == 0)
                {
                    return false;
                }
            }

            return true;
        }*/
    }
}