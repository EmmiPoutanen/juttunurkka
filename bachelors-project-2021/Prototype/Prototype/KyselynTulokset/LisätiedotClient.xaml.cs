
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

using System.Collections.ObjectModel;

namespace Prototype
{
    public partial class LisätiedotClient : ContentPage
    {
        public ResultsViewModel ViewModel { get; set; }

        public LisätiedotClient()
        {
            InitializeComponent();
            ViewModel = new ResultsViewModel();
            BindingContext = ViewModel;

            ProcessEmojiResults();
            ReceiveVote1();
        }

        private void ProcessEmojiResults()
        {
            var emojiResults = Main.GetInstance().client.summary.GetEmojiResults()
                                 .OrderByDescending(key => key.Value);
            Console.WriteLine(emojiResults.Count());

            int totalVotes = emojiResults.Sum(item => item.Value);

            foreach (var item in emojiResults)
            {
                var image = $"emoji{item.Key}lowres.png";
                var amount = item.Value;
                var scale = totalVotes == 0 ? 0 : (1.0 * amount / totalVotes) * 200; // Scale bars dynamically

                ViewModel.Results.Add(new ResultItem
                {
                    Image = image,
                    Amount = amount.ToString(),
                    Scale = (int)scale
                });
            }
        }

        private async void ReceiveVote1()
        {
            bool success = await Main.GetInstance().client.ReceiveVote1Candidates();
            if (success)
            {
                Console.WriteLine("Received Vote1 successfully");
                await Navigation.PushAsync(new AktiviteettiäänestysEka());
            }
        }

        async void PoistuClicked(object sender, EventArgs e)
        {
            var res = await DisplayAlert("Oletko varma että tahdot poistua kyselystä?", "", "Kyllä", "Ei");

            if (res)
            {
                Main.GetInstance().client.DestroyClient();
                await Navigation.PopToRootAsync();
            }
        }
    }

    public class ResultsViewModel
    {
        public ObservableCollection<ResultItem> Results { get; set; }

        public ResultsViewModel()
        {
            Results = new ObservableCollection<ResultItem>();
        }
    }

    public class ResultItem
    {
        public string Image { get; set; }
        public string Amount { get; set; }
        public int Scale { get; set; }
    }
}
