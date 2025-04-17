
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

    public partial class TulostenOdotus : ContentPage
    {
        
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

            Console.WriteLine("Starting activity vote");
            Main.GetInstance().host.StartActivityVote();

            
            //timer set to vote times, cooldowns, plus one extra
            _countSeconds = Main.GetInstance().host.voteCalc.vote1Timer + ( 3 * Main.GetInstance().host.voteCalc.coolDown);
            // TODO Xamarin.Forms.Device.StartTimer is no longer supported. Use Microsoft.Maui.Dispatching.DispatcherExtensions.StartTimer instead. For more details see https://learn.microsoft.com/en-us/dotnet/maui/migration/forms-projects#device-changes
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                _countSeconds--;

           //      timer.Text = _countSeconds.ToString();

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

        async protected override void OnAppearing()
        {
           
            base.OnAppearing();
            
            await UpdateProgressBar(0, 38000);
            
        }

        async Task UpdateProgressBar(double Progress, uint time)
        {
            await progressBar.ProgressTo(Progress, time, Easing.Linear);
            //siirtyy eteenpäin automaattisesti 45 sekunnin jälkeen
            if (progressBar.Progress == 0)
            {
                await Main.GetInstance().host.CloseSurvey();
                await Navigation.PushAsync(new AktiviteettiäänestysTulokset());
            }
        }

       
       
        protected override bool OnBackButtonPressed()
        {
            return true;

        }




    }
}