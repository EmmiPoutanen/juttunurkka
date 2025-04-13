
/*
Copyright 2021 Emma Kemppainen, Jesse Huttunen, Tanja Kultala, Niklas Arjasmaa
          2022 Pauliina Pihlajaniemi, Viola Niemi, Niina Nikki, Juho Tyni, Aino Reinikainen, Essi Kinnunen

This file is part of "Juttunurkka".

Juttunurkka is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, version 3 of the License.

Juttunurkka is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Juttunurkka.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace Prototype
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LuoKyselyToimenpiteetItkunauru : ContentPage
    {

        public IList<CollectionItem> Items { get; set; }
        private string myStringProperty;

        public string MyStringProperty
        {
            get { return myStringProperty; }
            set
            {
                myStringProperty = value;
                OnPropertyChanged(nameof(MyStringProperty)); // Notify that there was a change on this property
            }
        }

        public class CollectionItem

        {
            public Emoji Emoji { get; set; }
            public IList<string> ActivityChoises { get; set; }
            public ObservableCollection<object> Selected { get; set; }
            public CollectionItem(Emoji emoji, IList<string> activities)
            {
                Emoji = emoji;
                ActivityChoises = activities;

                if (!ActivityChoises.Contains("Luo oma vaihtoehto..."))
                {
                    ActivityChoises.Add("Luo oma vaihtoehto...");
                }

                Selected = new ObservableCollection<object>();
                foreach (var item in emoji.activities)
                {
                    if (ActivityChoises.Contains(item))
                        Selected.Add(item);
                }
            }
        }

        public LuoKyselyToimenpiteetItkunauru()
        {
            InitializeComponent();

            //alustus
            Items = new List<CollectionItem>();
            int numero = 0;
            foreach (var individual in SurveyManager.GetInstance().GetSurvey().emojis)
            {
                if (individual.Name == "Itkunauru")
                {
                    Items.Add(new CollectionItem(SurveyManager.GetInstance().GetSurvey().emojis[numero], Const.activities[6]));
                    break;
                }
                else
                {
                    numero++;
                }
            }
            BindingContext = this;

            int selectedEmojis = SurveyManager.GetInstance().GetSurvey().emojis.Count;
            int emojiNumber = numero + 1;
            String title = "Aktiviteetti " + emojiNumber + "/" + selectedEmojis;
            MyStringProperty = title;
        }

        async void EdellinenButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is CollectionView cv && cv.SelectionChangedCommandParameter is CollectionItem item)
            {
                var valittu = e.CurrentSelection.FirstOrDefault() as string;

                if (valittu == "Luo oma vaihtoehto...")
                {
                    cv.SelectedItems.Remove(valittu);

                    await Navigation.PushAsync(new Prototype.LuoKysely.LuoOmaVaihtoehto((syote) =>
                    {
                        if (!string.IsNullOrWhiteSpace(syote) && !item.ActivityChoises.Contains(syote))
                        {
                            item.ActivityChoises.Insert(item.ActivityChoises.Count - 1, syote);
                            item.Selected.Add(syote);
                        }
                    }));
                }
            }
        }

        async void JatkaButtonClicked(object sender, EventArgs e)
        {

            //error if not all emojis have at least 2 selected activity
            if (!ActivitiesSet())
            {
                await DisplayAlert("Kaikkia valintoja ei ole tehty", "Sinun on valittava vähintään kaksi aktiviteettia", "OK");
                return;
            }

            //asetetaan emojit survey olioon
            List<Emoji> tempEmojis = new List<Emoji>();
            List<string> tempActivities = new List<string>();

            foreach (var item in Items)
            {
                foreach (var selection in item.Selected)
                {
                    tempActivities.Add(selection as string);
                }
                item.Emoji.activities = tempActivities;
                tempEmojis.Add(item.Emoji);
            }
            int numero = 0;
            foreach (var individual in SurveyManager.GetInstance().GetSurvey().emojis)
            {
                if (individual.Name == "Itkunauru")
                {
                    SurveyManager.GetInstance().GetSurvey().emojis[numero].activities = tempActivities;
                    break;
                }
                else
                {
                    numero++;
                }
            }
            int nextEmojiNumber = numero + 1;
            int luku = SurveyManager.GetInstance().GetSurvey().emojis.Count;

            if (nextEmojiNumber < luku)
            {
                String name = SurveyManager.GetInstance().GetSurvey().emojis[nextEmojiNumber].Name;

                if (name == "Iloinen")
                {
                    await Navigation.PushAsync(new LuoKyselyToimenpiteetIloinen());
                }
                else if (name == "Hämmästynyt")
                {
                    await Navigation.PushAsync(new LuoKyselyToimenpiteetHammastynyt());
                }
                else if (name == "Neutraali")
                {
                    await Navigation.PushAsync(new LuoKyselyToimenpiteetNeutraali());
                }
                else if (name == "Vihainen")
                {
                    await Navigation.PushAsync(new LuoKyselyToimenpiteetVihainen());
                }
                else if (name == "Väsynyt")
                {
                    await Navigation.PushAsync(new LuoKyselyToimenpiteetVasynyt());
                }
                else if (name == "Miettivä")
                {
                    await Navigation.PushAsync(new LuoKyselyToimenpiteetMiettiva());
                }
                else if (name == "Itkunauru")
                {
                    await Navigation.PushAsync(new LuoKyselyToimenpiteetItkunauru());
                }
                else { }
            }
            else
            {
                await Navigation.PushAsync(new LuoKyselyLopetus());
            }
        }

        //function which checks whether the user has selected at least 2 activities.
        private bool ActivitiesSet()
        {
            foreach (var item in Items)
            {
                if (item.Selected.Count < 2)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
