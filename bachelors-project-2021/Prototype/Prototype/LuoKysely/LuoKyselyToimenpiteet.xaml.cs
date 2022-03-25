
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
            await Navigation.PushAsync(new LuoKyselyEmojit());
        }

        //valintanapit ensin disabled.

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
        
        void Button1Clicked(object sender, EventArgs e)
        {
            MinButton.IsEnabled = true;
        }

        void Button2Clicked(object sender, EventArgs e)
        {
            TauluButton.IsEnabled = true;
        }

        void Button3Clicked(object sender, EventArgs e)
        {
            HirsiButton.IsEnabled = true;
        }



        void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			if (sender is CollectionView cv && cv.SelectionChangedCommandParameter is CollectionItem item)
			{
                
			}
        }
        

        async void JatkaButtonClicked(object sender, EventArgs e)
        {
            /*
			//error if not all emojis have at least 1 selected activity
			if (!ActivitiesSet())
			{
                await DisplayAlert("Kaikkia valintoja ei ole tehty", "Sinun on valittava jokaiselle mielialalle vähintään yksi aktiviteetti", "OK");
                return;
            }*/

            //asetetaan emojit survey olioon
            List<Emoji> tempEmojis = new List<Emoji>();
			foreach (var item in Items)
			{
                List<string> tempActivities = new List<string>();
				foreach (var selection in item.Selected)
				{
                    tempActivities.Add(selection as string);
				}
                item.Emoji.activities = tempActivities;
                tempEmojis.Add(item.Emoji);
			}
            SurveyManager.GetInstance().GetSurvey().emojis = tempEmojis;            

            // siirrytään "Luo kysely -lopetus" sivulle 
            await Navigation.PushAsync(new LuoKyselyLopetus()); ;
        }

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
		}
    }
}