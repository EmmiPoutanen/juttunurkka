
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
    public partial class OdotetaanOsallistujiaOpettaja : ContentPage
    {
        public string roomCode { get; set; } = "Roomcode: ";

        //public int clientCount { get; set; } = 0;

        public OdotetaanOsallistujiaOpettaja()
        {
            InitializeComponent();
            roomCode += SurveyManager.GetInstance().GetSurvey().RoomCode;
            BindingContext = this; 

            //Ei enää mahdollista päästä takaisin kysleyn luontiin painamalla navigoinnin backbuttonia 
            NavigationPage.SetHasBackButton(this, false);

            //actually run the survey
            //Host();
        }

        private async void Host()
		{
            //hakee osallistujien määrän?
            //clientCount = Main.GetInstance().host.clientCount;
            
            if (!await Main.GetInstance().HostSurvey())
            {
                //host survey ended in a fatal unexpected error, aborting survey.
                //pop to root and display error
                await Navigation.PopToRootAsync();
                await DisplayAlert("Kysely suljettiin automaattisesti", "Tapahtui odottamaton virhe.", "OK");
            }
        }

        private async void AloitaButtonClicked(object sender, EventArgs e)
        {
            //siirrytään odottamaan vastauksia
            await Navigation.PushAsync(new OdotetaanVastauksiaOpe());
            //siirrä tämä seuraavalle sivulle
            /*
			//Back to main and error, if nobody joined the survey!
			if (Main.GetInstance().host.clientCount == 0)
			{
                Main.GetInstance().host.DestroyHost();
                await Navigation.PopToRootAsync();
                await DisplayAlert("Kysely suljettiin automaattisesti", "Kyselyyn ei saatu yhtään vastausta", "OK");
                return;
            }

            await Main.GetInstance().host.CloseSurvey();
            await Navigation.PushAsync(new TabbedViewHost());
        }

        //Device back button disabled
        protected override bool OnBackButtonPressed()
        {
            return true;
        */
        }

        private async void KeskeytaButtonClicked(object sender, EventArgs e)
        {

            //Varmistu kyselyn peruuttamisen yhteydessä keskeyttää

            var res = await DisplayAlert("Oletko varma että tahdot keskeyttää kyselyn?", "", "Kyllä", "Ei");

            if (res == true) {
                await Navigation.PopToRootAsync();
            }
            else return; 
           

        }
    }
}