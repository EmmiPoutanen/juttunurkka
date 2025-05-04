
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
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using System.Collections.ObjectModel;

namespace Prototype
{
    /// <summary>
    /// Class for displaying results on UI
    /// </summary>
    public class VoteResultViewModel
    {
        public string Image { get; set; } = "";
        public string Title { get; set; } = "";
        public int Amount { get; set; }
        public double Scale { get; set; }
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AktiviteettiäänestysTulokset : ContentPage
    {
        public ObservableCollection<VoteResultViewModel> Results { get; set; }

        public AktiviteettiäänestysTulokset()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);

            Results = new ObservableCollection<VoteResultViewModel>();

            Dictionary<Activity, int> voteResults;
            if (Main.GetInstance().state == Main.MainState.Participating)
            {
                voteResults = Main.GetInstance().client.voteResult;
            }
            else
            {
                voteResults = Main.GetInstance().host.data.vote1Results;
            }

            // Transform voteResults into Results
            int maxVotes = voteResults.Values.Count != 0 ? voteResults.Values.Max() : 1;
            foreach (var kvp in voteResults.OrderByDescending(kvp => kvp.Value))
            {
                Results.Add(new VoteResultViewModel
                {
                    Image = kvp.Key.ImageSource,
                    Title = kvp.Key.Title,
                    Amount = kvp.Value,
                    Scale = (kvp.Value / (double)maxVotes) * 200
                });
            }

            // Always add the "vastaamatta" bar TODO: Get the value from somewhere
            Results.Add(new VoteResultViewModel
            {
                Image = "",
                Title = "Vastaamatta",
                Amount = 0,
                Scale = (0 / (double)maxVotes) * 100
            });

            BindingContext = this;
        }

        async void SuljeClicked(object sender, EventArgs e)
        {
			if (Main.GetInstance().state == Main.MainState.Participating)
			{
                Main.GetInstance().client.DestroyClient();
			} else {
                Main.GetInstance().host.DestroyHost();
			}
            await Navigation.PushAsync(new JuttunurkkaSuljettu());
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