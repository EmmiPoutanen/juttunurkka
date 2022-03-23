
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Prototype
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AktiviteettiäänestysToka : ContentPage
    {

        private int _countSeconds = 10;

        public IList<string> Items { get; set; }
        public string Selected { get; set; } = null;
        public AktiviteettiäänestysToka()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);

            //alustus
            Items = Main.GetInstance().client.voteCandidates2;

            BindingContext = this;

            Vote2();
        }
        private async void Vote2() {
            _countSeconds = Main.GetInstance().client.vote2Time;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                _countSeconds--;

                timer.Text = _countSeconds.ToString();


                if (_countSeconds == 0)
                {
                  
                        return false;
              


                }

                return Convert.ToBoolean(_countSeconds);
            });

            await Task.Delay(Main.GetInstance().client.vote2Time * 1000);

            if (Selected != null)
            {
                await Main.GetInstance().client.SendVote2Result(Selected);
            }

            bool success = await Main.GetInstance().client.ReceiveVoteResult();
            if (success)
            {
                //received result changing view
                await Navigation.PushAsync(new AktiviteettiäänestysTulokset());
            }
        }


        //Device back button disabled
        protected override bool OnBackButtonPressed()
        {
            return true;

        }
    }
}