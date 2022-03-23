
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

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Prototype
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmojinValinta : ContentPage
    {
        public string introMessage { get; set; }
        private int answer;

        public EmojinValinta()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);

            introMessage = Main.GetInstance().client.intro;

            BindingContext = this;
        }

        //Device back button disabled
        protected override bool OnBackButtonPressed()
        {
            return true;

        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            b0.Scale = 1;
            b1.Scale = 1;
            b2.Scale = 1;
            b3.Scale = 1;
            b4.Scale = 1;
            b5.Scale = 1;
            b6.Scale = 1;

            ImageButton emoji = sender as ImageButton;
            emoji.Scale = 1.75;
            answer = int.Parse(emoji.ClassId.ToString());
            Console.WriteLine(answer);
            Vastaus.IsEnabled = true;
        }

        private async void Vastaa_Clicked(object sender, EventArgs e)
        {
            await Main.GetInstance().client.SendResult(answer.ToString());
            await Navigation.PushAsync(new OdotetaanVastauksiaClient());
        }
    }
}