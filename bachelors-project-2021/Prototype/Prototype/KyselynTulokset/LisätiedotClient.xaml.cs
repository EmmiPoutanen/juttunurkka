
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
    public partial class LisätiedotClient : ContentPage
    {
        public IList<string> resultImages { get; set; }
        public IList<double> resultScale { get; set; }
        public IList<int> resultAmount { get; set; }
        public LisätiedotClient()
        {
            InitializeComponent();
            resultImages = new List<string>();
            resultScale = new List<double>();
            resultAmount = new List<int>();

            int count = 0;
            double calculateScale = 0.0;
            Dictionary<int, int> sorted = new Dictionary<int, int>();
            foreach (KeyValuePair<int, int> item in Main.GetInstance().client.summary.GetEmojiResults().OrderByDescending(key => key.Value))
            {
                sorted.Add(item.Key, item.Value);
                resultAmount.Add(item.Value);
                count += item.Value;
            }
            foreach (int key in sorted.Keys)
            {
                resultImages.Add("emoji" + key.ToString() + "lowres.png");
            }
            foreach (int value in sorted.Values)
            {
                if (count == 0)
                {
                    resultScale.Add(0);
                }
                else
                {
                    calculateScale = 1 * (double)value / count;
                    resultScale.Add(calculateScale);
                }
            }

            BindingContext = this;
        }
        async void PoistuClicked(object sender, EventArgs e)
        {

            // Varmistus kyselystä poistumisen yhteydessä

            var res = await DisplayAlert("Oletko varma että tahdot poistua kyselystä?", "", "Kyllä", "Ei");

            if (res == true)
            {
                Main.GetInstance().client.DestroyClient();
                await Navigation.PopToRootAsync();
            }
            else return;
           
        }
    }
}