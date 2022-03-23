
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

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System;
using System.Threading.Tasks;

namespace Prototype
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabbedViewClient : TabbedPage
    {
        public TabbedViewClient()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            ReceiveVote1();
        }
        private async void ReceiveVote1()
        {
            bool success = await Main.GetInstance().client.ReceiveVote1Candidates();
			if (success)
			{
                Console.WriteLine("Received Vote1 successfully");
				await Navigation.PushAsync(new AktiviteettiäänestysEka());
			}
            //Did not get candidates, now it is safe to leave this survey, because nothing more is going to happen
            //TODO maybe indicate that the survey has concluded somehow?? e.g "Poistu" turns green
            return;
        }

        //Device back button disabled
        protected override bool OnBackButtonPressed()
        {
            return true;

        }
    }
}