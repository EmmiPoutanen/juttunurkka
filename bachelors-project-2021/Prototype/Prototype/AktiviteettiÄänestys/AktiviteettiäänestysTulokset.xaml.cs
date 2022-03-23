
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
    public partial class AktiviteettiäänestysTulokset : ContentPage
    {
        public AktiviteettiäänestysTulokset()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            if (Main.GetInstance().state == Main.MainState.Participating)
            {
                result.Text = Main.GetInstance().client.voteResult;
            }
            else
            {
                result.Text = Main.GetInstance().host.data.voteResult;
            }
            
        }

        async void PoistuClicked(object sender, EventArgs e)
        {
			if (Main.GetInstance().state == Main.MainState.Participating)
			{
                Main.GetInstance().client.DestroyClient();
			} else {
                Main.GetInstance().host.DestroyHost();
			}
            await Navigation.PushAsync(new MainPage());
        }


        // Device back button navigation 
        protected override bool OnBackButtonPressed()
        {

            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await DisplayAlert("Poistutaanko tulosten tarkastelusta ? ","","Kyllä", "Ei"))
                {
                    base.OnBackButtonPressed();
                    if (Main.GetInstance().state == Main.MainState.Participating)
                    {
                        Main.GetInstance().client.DestroyClient();
                    }
                    else
                    {
                        Main.GetInstance().host.DestroyHost();
                    }
                    await Navigation.PushAsync(new MainPage());
                }
              
            });

            return true;


        
       
        }
    }
}