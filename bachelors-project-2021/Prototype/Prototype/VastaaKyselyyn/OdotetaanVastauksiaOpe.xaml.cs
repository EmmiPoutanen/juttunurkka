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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace Prototype
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OdotetaanVastauksiaOpe : ContentPage
    {

        CancellationTokenSource cts;
        public string RoomCode { get; set; }
        public OdotetaanVastauksiaOpe()
        {
            InitializeComponent();
            // Set as true for testing
            NavigationPage.SetHasBackButton(this, true);
            Survey s = SurveyManager.GetInstance().GetSurvey();
            RoomCode = s.RoomCode;
            BindingContext = this;

            Host();
        }

        private async void Host()
        {

            if (!await Main.GetInstance().HostSurvey())
            {
                //host survey ended in a fatal unexpected error, aborting survey.
                //pop to root and display error
                await Navigation.PopToRootAsync();
                await DisplayAlert("Kysely suljettiin automaattisesti3", "Tapahtui odottamaton virhe.", "OK");
            }
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
            await Navigation.PushAsync(new LisätiedotHost());
        }
    }
    }
