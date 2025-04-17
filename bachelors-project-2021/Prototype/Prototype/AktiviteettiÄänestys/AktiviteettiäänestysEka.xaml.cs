﻿
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Button = Microsoft.Maui.Controls.Button;

namespace Prototype
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AktiviteettiäänestysEka : ContentPage
	{
		private int _countSeconds = 10;
        private const double DefaultBorderWidth = 0;
        private const double SelectedBorderWidth = 6;
        private Button _selectedButton;
        private readonly QuestionToSpeech _questionToSpeechClient = new();

        public IList<CollectionItem> Items { get; set; }
		public class CollectionItem
		{
			public int ID;
			public string ImageSource { get; set; }
			public IList<Activity> ActivityChoises { get; set; }
			public string Selected { get; set; }

            public CollectionItem(int ID, string ImageSource, IList<Activity> ActivityChoises)
			{
				this.ID = ID;
				this.ImageSource = ImageSource;
				this.ActivityChoises = ActivityChoises;
				this.Selected = null;
			}
		}

		public AktiviteettiäänestysEka()
		{
			NavigationPage.SetHasBackButton(this, false);
			InitializeComponent();

			//alustus
			Items = new List<CollectionItem>();
			string img;

			Console.WriteLine("Setting vote 1 page content");
			foreach (var item in Main.GetInstance().client.voteCandidates1)
			{
				Console.WriteLine("Key: {0}, Value: {1}", item.Key, item.Value);
				img = "emoji" + item.Key.ToString() + ".png";
				Items.Add(new CollectionItem(item.Key, img, item.Value));
            }

			BindingContext = this;

			Vote1();
            ActivityButton1.Clicked += OnButton1Clicked;
            ActivityButton2.Clicked += OnButton2Clicked;
        }

        private async void OnButton1Clicked(object sender, EventArgs e)
        {
            SelectButton(ActivityButton1);
            var title = Items[0].ActivityChoises[0].Title;
            if (!string.IsNullOrEmpty(title))
            {
                await _questionToSpeechClient.Speak(title);
            }
        }

        private async void OnButton2Clicked(object sender, EventArgs e)
        {
            SelectButton(ActivityButton2);
            var title = Items[0].ActivityChoises[1].Title;
            if (!string.IsNullOrEmpty(title))
            {
                await _questionToSpeechClient.Speak(title);
            }
        }

        private void SelectButton(Button selectedButton)
        {
            // Reset previously selected button
            if (_selectedButton != null)
            {
                _selectedButton.BorderWidth = DefaultBorderWidth;
            }

            // Set new selected button
            _selectedButton = selectedButton;
            _selectedButton.BorderWidth = SelectedBorderWidth;
            SaveButton.IsEnabled = true;
        }

        private async void Vote1()
		{

			_countSeconds = Main.GetInstance().client.vote1Time;
			// TODO Xamarin.Forms.Device.StartTimer is no longer supported. Use Microsoft.Maui.Dispatching.DispatcherExtensions.StartTimer instead. For more details see https://learn.microsoft.com/en-us/dotnet/maui/migration/forms-projects#device-changes
			Device.StartTimer(TimeSpan.FromSeconds(1), () =>
		   {
			   _countSeconds--;

			   //timer.Text = _countSeconds.ToString();


			   if (_countSeconds == 0)
			   {

				   return false;

			   }

			   return Convert.ToBoolean(_countSeconds);
		   });

			await Task.Delay(Main.GetInstance().client.vote1Time * 1000);

            await SendActivityVote();
            await Navigation.PushAsync(new AktiviteettiäänestysTulokset());

		}

		private async void SaveAnswer(object sender, EventArgs e)
		{
            if (_selectedButton == null)
            {
                Console.WriteLine("No activity selected");
                return;
            }

            await SendActivityVote();
            await Navigation.PushAsync(new AktiviteettiäänestysTulokset());
        }

        private async Task SendActivityVote()
        {
            var currentItem = Items[0];
            Activity answer = null;

            if (_selectedButton == ActivityButton1)
            {
                answer = currentItem.ActivityChoises[0];
            }
            else if (_selectedButton == ActivityButton2)
            {
                answer = currentItem.ActivityChoises[1];
            }

            if (answer != null)
            {
                // TODO: Should send the correct answer, need to change the format in SendVote1Result()
                var dummyAnswer = new Dictionary<int, string>
                {
                    { 1, "Activity Name" } // Replace with realistic test data
				};
                await Main.GetInstance().client.SendVote1Result(dummyAnswer);
            }
            else
            {
                Console.WriteLine("Something went wrong with activity selection.");
            }
        }
	}
}