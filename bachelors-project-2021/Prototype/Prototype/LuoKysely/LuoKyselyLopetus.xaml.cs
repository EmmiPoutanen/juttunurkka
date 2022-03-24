
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Prototype
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LuoKyselyLopetus : ContentPage
    {
        public string introMessage { get; set; } = "Kysymys: ";
        public LuoKyselyLopetus()
        {
            InitializeComponent();
            Survey s = SurveyManager.GetInstance().GetSurvey();
            introMessage += s.introMessage;
            BindingContext = this;

            //NameEditor.Text = SurveyManager.GetInstance().GetSurvey().Name;
            //KeyEditor.Text = SurveyManager.GetInstance().GetSurvey().RoomCode;
        }

        async void EdellinenButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LuoKyselyToimenpiteet());
        }

        async void JatkaButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Esikatselu());
        }
        /*
        async void TallennaJaPoistuClicked(object sender, EventArgs e)
        {
            
            if (NameEditor != null && !string.IsNullOrEmpty(NameEditor.Text) && KeyEditor != null && !string.IsNullOrEmpty(KeyEditor.Text) )
            {

                SurveyManager man = SurveyManager.GetInstance();
                //save survey code and name
                man.GetSurvey().RoomCode = KeyEditor.Text;
                man.GetSurvey().Name = NameEditor.Text;
                //save survey
                man.SaveSurvey(NameEditor.Text + ".txt");

                // siirrytään etusivulle 
                await Navigation.PushAsync(new MainPage());

            }

            else await DisplayAlert("Nimi tai avainkoodi puuttuu", "Sinun on asetettava kyselylle nimi ja avainkoodi", "OK");
            
        }*/

        /*
        async void PeruutaClicked(object sender, EventArgs e)
        {

            var res = await DisplayAlert("Tahdotko varmasti keskeytää kyselyn luonnin?", "", "Kyllä", "Ei");

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
        }*/

        void JaaClicked(object sender, EventArgs e)
        {


            /*if(NameEditor.Text != null && KeyEditor.Text != null) {

                // Kysytään kyselyn tallentamisesta
                popupSelection.IsVisible = true; 
            }

            else DisplayAlert("Nimi tai avainkoodi puuttuu", "Sinun on asetettava kyselylle nimi ja avainkoodi", "OK");
            */
        }

        void X_Clicked(object sender, EventArgs e)
        {

           // Suljetaan popup
            //popupSelection.IsVisible = false;

        }

        async void Ei_Clicked(object sender, EventArgs e)
        {
            // siirrytään OdotettaanVastauksia, ei tallenneta kyselyä
            await Navigation.PushAsync(new OdotetaanVastauksia());
        }

        /*
        async void Kyllä_Clicked(object sender, EventArgs e)
        {

            //kyselyn tallennus!

            
            SurveyManager man = SurveyManager.GetInstance();
            //save survey code
            man.GetSurvey().RoomCode = KeyEditor.Text;
            man.GetSurvey().Name = NameEditor.Text;
            //save survey
            man.SaveSurvey(NameEditor.Text + ".txt");

            // siirrytään OdotettaanVastauksia sivulle 
            await Navigation.PushAsync(new OdotetaanVastauksia());
        }*/
        private static void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            // jos tekstikenttä ei ole tyhjä
            if (!string.IsNullOrWhiteSpace(args.NewTextValue))
            {
                //isValid bool true/false arvo tarkastetaan käymällä läpi syötetyt characterit, että onko ne kirjaimia tai numeroita
                bool isValid = args.NewTextValue.ToCharArray().All(x => char.IsLetterOrDigit(x));
                
                //jos isValid on false, niin poistetaan kirjain heti, kun se kirjoitetaan
                ((Entry)sender).Text = isValid ? args.NewTextValue : args.NewTextValue.Remove(args.NewTextValue.Length - 1);
            }
        }
    }
}