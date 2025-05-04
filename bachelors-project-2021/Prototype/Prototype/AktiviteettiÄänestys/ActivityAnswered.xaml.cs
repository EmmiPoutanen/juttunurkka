
/*
Copyright 2021 Emma Kemppainen, Jesse Huttunen, Tanja Kultala, Niklas Arjasmaa
          2022 Pauliina Pihlajaniemi, Viola Niemi, Niina Nikki, Juho Tyni, Aino Reinikainen, Essi Kinnunen
          2025 Emmi Poutanen

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

namespace Prototype
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ActivityAnswered : ContentPage
	{
        public Activity ActivityToShow { get; set; }

        private readonly Activity? _votedAcitivy;
		private readonly int _remainingTime;

		public ActivityAnswered(Activity? votedActivity, int remainingTime)
		{
			_votedAcitivy = votedActivity;
			_remainingTime = remainingTime;
			NavigationPage.SetHasBackButton(this, false);
            ActivityToShow = votedActivity ?? new Activity();
            InitializeComponent();
            BindingContext = this;

            StartCountdown();
        }

        /// <summary>
        /// Update the progressbar and countdown when the voting should be closing.
        /// </summary>
        private async void StartCountdown()
        {
            int totalSeconds = _remainingTime;
            int elapsed = 0;

            while (elapsed < totalSeconds)
            {
                double progress = 1.0 - (double)elapsed / totalSeconds;
                progressBar.Progress = progress;
                await Task.Delay(1000);
                elapsed++;
            }

            progressBar.Progress = 0;

            await OnCountdownFinished();
        }

        private async Task OnCountdownFinished()
        {
            // Try to read the results when countdown is finished
            // TODO: Can the host cancel the voting?
            bool success = await Main.GetInstance().client.ReceiveVoteResult();
            if (success)
            {
                //received result changing view
                await Navigation.PushAsync(new AktiviteettiäänestysTulokset());
                return;
            }
            await DisplayAlert("VIRHE", "Tulosten haku epäonnistui", "OK");
        }
    }
}