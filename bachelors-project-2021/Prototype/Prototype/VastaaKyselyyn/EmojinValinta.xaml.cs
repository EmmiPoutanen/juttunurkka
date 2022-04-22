
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
     //   public string emoji1 { get; set; }
     //   public string emojiNimetString { get; set; }
     //   public List<string> emojiNames { get; set; }

        private int answer;

        public IList<CollectionItem> Emojit { get; private set; } = null;
       

        public class CollectionItem 
        {
            public Emoji Item { get; set; } = null;
            public int ID;
            public string ImageSource { get; set; }
            public string Name { get; set; }
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

            /*    string emojiNimetString = Main.GetInstance().client.emojinamesTogetherAsString;

               string [] emojinimetlista= emojiNimetString.Split(',');
                foreach(string emojinimi in emojinimetlista)
                {
                    emojiNames.Add(emojinimi);
                    if (emojinimi == ",")
                    {
                        emojiNames.Remove(emojinimi);
                    }
                }
            */
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
                //NEW APPROACH TO DELETE THE SURVEYMANAGER FROM CLIENT--try this!
//                trying to solve this puzzle:

  /*              foreach(var emojistring in emojinimetlista)
                {
                    CollectionItem i = new CollectionItem();
                    i.Item.Name= emojistring;
                    Emojit.Add(i);
                }*/

                //string[] emoji2 = Main.GetInstance().client.emoji1.Split(',');
                /*   foreach(string emojinimi1 in emojiNames)
                   {
                       if(i.Item.Name == emojinimi1)
                       {
                           Emojit.Add(i);

                       }
                       if (emojiNames.Count == Emojit.Count)
                       {
                           break;
                       }
                   }*/




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
 
            if (sender is ImageButton b && b.Parent is Grid g && g.Children[0] is Frame f)
            {
                // change the text of the button to the answer
                CollectionView view = (f.Children[0] as StackLayout).Children[0] as CollectionView;
            }

            //Tallennetaan vastaus
            answer = int.Parse(emoji.ClassId.ToString());

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