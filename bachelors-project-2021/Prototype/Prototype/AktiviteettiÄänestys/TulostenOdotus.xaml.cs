
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
    public partial class TulostenOdotus : ContentPage
    {

        private int _countSeconds = 10;

        public TulostenOdotus()
        {
            InitializeComponent();

            //poistetaan turha navigointipalkki
            NavigationPage.SetHasNavigationBar(this, false);

            Main.GetInstance().host.StartActivityVote();

            //timer set to vote times, cooldowns, plus one extra
            _countSeconds = Main.GetInstance().host.voteCalc.vote1Timer + Main.GetInstance().host.voteCalc.vote2Timer + ( 3 * Main.GetInstance().host.voteCalc.coolDown);
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                _countSeconds--;

                 timer.Text = _countSeconds.ToString();

				if (Main.GetInstance().host.isVoteConcluded)
				{
                    Navigation.PushAsync(new AktiviteettiäänestysTulokset());
                    return false;
                }
                    

                 if (_countSeconds == 0) {
                    Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                    {
                        return false;
                    });

                  
                 }

                return Convert.ToBoolean(_countSeconds);
            });
        }

        //Device back button disabled
        protected override bool OnBackButtonPressed()
        {
            return true;

        }




    }
}