﻿
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
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Prototype
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmojinValinta : ContentPage
    {
        CancellationTokenSource cts;
        public string introMessage { get; set; }
  

        private int answer;

        public IList<CollectionItem> Emojit { get; private set; } = null;
       

        public class CollectionItem 
        {
            public Emoji Item { get; set; } = null;

            public string Selected { get; set; }

            public CollectionItem()
            {
                this.Selected = null;
            }

        }

        public EmojinValinta()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);


            Emojit = new List<CollectionItem>();
            List<Emoji> temp = SurveyManager.GetInstance().GetSurvey().emojis;

            string emoji2 = Main.GetInstance().client.emoji1;
            string[] emojinimetlista = emoji2.Split(',');
            List<string> emojiNames = new List<string>();

            foreach (var emojinimi in emojinimetlista)
            {
                if (emojinimi != ",")
                {
                    emojiNames.Add(emojinimi);
                }
            }

           
            foreach (var item in temp)
            {
                CollectionItem i = new CollectionItem();
                i.Item = item;
                foreach (var emojistring in emojinimetlista)
                {
                    if (i.Item.Name == emojistring)
                    {
                        Emojit.Add(i);
                    }
                }
                

            }
            
            introMessage = Main.GetInstance().client.intro;

            BindingContext = this;
        }

        async protected override void OnAppearing()
        {
            cts = new CancellationTokenSource();
            var token = cts.Token;
            base.OnAppearing();

            try
            {
                await UpdateProgressBar(0, 60000, token);
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine("Task cancelled", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("ex {0}", e.Message);
            }
            finally
            {
                cts.Dispose();
            }

        }

        async Task UpdateProgressBar(double Progress, uint time, CancellationToken token)
        {

            await progressBar.ProgressTo(Progress, time, Easing.Linear);
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }
            //siirtyy eteenpäin automaattisesti 60 sekunnin jälkeen
            if (progressBar.Progress == 0)
            {
                await Main.GetInstance().host.CloseSurvey();
                await Navigation.PushAsync(new LisätiedotHost());
            }

        }

        //Device back button disabled
        protected override bool OnBackButtonPressed()
        {
            return true;

        }


        private void Button_Clicked(object sender, EventArgs e)
        {

            ImageButton emoji = sender as ImageButton;

            // Tarkoitus saada päivitetty näkymää niin, että vain yksi kuva kerralaan on valittuna isoksi
            /* if (sender is ImageButton b && b.Parent is Grid g && g.Children[0] is Frame f)
             {
                 // change the text of the button to the answer
                 CollectionView view = (f.Children[0] as StackLayout).Children[0] as CollectionView;
             }*/

            //Tallennetaan vastaus
            answer = int.Parse(emoji.ClassId.ToString());

            // Nyt kaikki, jotka valitaan, muutetaan isommaksi.
            emoji.Scale = 1.75;

            // Tarkistetaan, että vaan yhden valihtee
            Console.WriteLine("valittu " + answer);
            Vastaus.IsEnabled = true;

        }

        private async void Vastaa_Clicked(object sender, EventArgs e)
        {
            cts.Cancel(); //cancel task if button clicked
            await Main.GetInstance().client.SendResult(answer.ToString());
            await Navigation.PushAsync(new OdotetaanVastauksiaClient());
        }
    }
}