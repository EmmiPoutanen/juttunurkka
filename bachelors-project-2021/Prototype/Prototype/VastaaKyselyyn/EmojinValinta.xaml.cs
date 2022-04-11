
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

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Prototype
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmojinValinta : ContentPage
    {
        public string introMessage { get; set; }
        public string emoji1 { get; set; }
        private int answer;

        // <---
        public IList<CollectionItem> Emojit { get; private set; } = null;
        // --->

        //Haettu esikatseluosiosta apuun
        //<---
        public class CollectionItem
        {
            public Emoji Item { get; set; } = null;
        }
        //--->

        public EmojinValinta()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);

            //<---
           // Survey s = SurveyManager.GetInstance().GetSurvey();
//            introMessage += s.introMessage; // VAIKO TÄMÄ INTRO MESSAGE??

            Emojit = new List<CollectionItem>();
            List<Emoji> temp = SurveyManager.GetInstance().GetSurvey().emojis;

            int numero = 0;

            foreach (var item in temp)
            {
                CollectionItem i = new CollectionItem();
                i.Item = item;
                //string[] emoji2 = Main.GetInstance().client.emoji1.Split(',');
                string emoji2 = Main.GetInstance().client.emoji1;
                if (i.Item.Name == emoji2)
                {
                    Emojit.Add(i);
                    numero++;
                }
            }
            //--->

            // Onko ero mainin kautta haettaessa vs. surveyn?
            introMessage = Main.GetInstance().client.intro;

            BindingContext = this;
        }

        //Device back button disabled
        protected override bool OnBackButtonPressed()
        {
            return true;

        }


        private void Button_Clicked(object sender, EventArgs e)
        {

            ImageButton emoji = sender as ImageButton;

            emoji.Scale = 1.75;
            answer = int.Parse(emoji.ClassId.ToString());

            // Tarkistetaan, että vaan yhden valihtee
            Console.WriteLine("valittu " + answer);
            Vastaus.IsEnabled = true;

        }

        private async void Vastaa_Clicked(object sender, EventArgs e)
        {
            await Main.GetInstance().client.SendResult(answer.ToString());
            await Navigation.PushAsync(new OdotetaanVastauksiaClient());
        }
    }
}