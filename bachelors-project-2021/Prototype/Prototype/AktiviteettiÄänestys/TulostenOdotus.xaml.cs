
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
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Prototype
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class TulostenOdotus : ContentPage
    {
        CancellationTokenSource cts;
        public string RoomCode { get; set; }
        private int _countSeconds = 10;

        public TulostenOdotus()
        {
            InitializeComponent();
            Survey s = SurveyManager.GetInstance().GetSurvey();
            RoomCode = s.RoomCode;
            BindingContext = this;

            //poistetaan turha navigointipalkki
            NavigationPage.SetHasNavigationBar(this, false);


            Main.GetInstance().host.StartActivityVote();

            //timer set to vote times, cooldowns, plus one extra
            /*_countSeconds = Main.GetInstance().host.voteCalc.vote1Timer + Main.GetInstance().host.voteCalc.vote2Timer + ( 3 * Main.GetInstance().host.voteCalc.coolDown);
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                _countSeconds--;

                 //timer.Text = _countSeconds.ToString();

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
            });*/
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
                await Navigation.PushAsync(new AktiviteettiäänestysTulokset());
            }
        }

        private async void LopetaClicked(object sender, EventArgs e)
        {
            //Back to main and error, if nobody joined the survey!
            /*if (Main.GetInstance().host.clientCount == 0)
            {
                Main.GetInstance().host.DestroyHost();
                await Navigation.PopToRootAsync();
                await DisplayAlert("Kysely suljettiin automaattisesti", "Kyselyyn ei saatu yhtään vastausta", "OK");
                return;
            }*/

            cts.Cancel(); //cancel task if button clicked
            await Main.GetInstance().host.CloseSurvey();
            await Navigation.PushAsync(new AktiviteettiäänestysTulokset());
        }

        //Device back button disabled
        protected override bool OnBackButtonPressed()
        {
            return true;

        }




    }
}