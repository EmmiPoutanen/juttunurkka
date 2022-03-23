
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
    public partial class OdotetaanVastauksiaClient : ContentPage
    {
        public OdotetaanVastauksiaClient()
        {
            InitializeComponent();
            //Ei enää mahdollista päästä takaisin kysleyn luontiin painamalla navigoinnin backbuttonia 
            NavigationPage.SetHasBackButton(this, false);
            ReceiveSurveyData();
        }
        private async void ReceiveSurveyData() {
            bool success = await Main.GetInstance().client.ReceiveSurveyDataAsync();
			if (success)
			{
                await Navigation.PushAsync(new TabbedViewClient());
			} else {
                Main.GetInstance().client.DestroyClient();
                await Navigation.PopToRootAsync();
            }
        }        

        private async void Poistu(object sender, EventArgs e)
        {

            // Varmistus kyselystä poistumisen yhteydessä

             var res = await DisplayAlert("Oletko varma että tahdot poistua kyselystä?", "", "Kyllä", "Ei");

            if (res == true)
            {
                Main.GetInstance().client.DestroyClient();
                await Navigation.PopToRootAsync();
            }
            else return;
            

        }

        //Device back button disabled
        protected override bool OnBackButtonPressed()
        {
            return true;

        }
    }
}