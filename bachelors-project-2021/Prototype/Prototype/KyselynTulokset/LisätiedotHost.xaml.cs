
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
    public partial class LisätiedotHost : ContentPage
    {
        public IList<CollectionItem> Emojit { get; private set; } = null;
        public IList<string> resultImages { get; set; }
        public string introMessage { get; set; }
        public IList<double> resultScale { get; set; }
        public IList<int> resultAmount { get; set; }
        public class CollectionItem
        {
            public Emoji Item { get; set; } = null;
        }
        public LisätiedotHost()
        {
            InitializeComponent();
            resultImages = new List<string>();
            resultScale = new List<double>();
            resultAmount = new List<int>();

            Survey s = SurveyManager.GetInstance().GetSurvey();
            introMessage += s.introMessage;

            Emojit = new List<CollectionItem>();
            List<Emoji> temp = s.emojis;

            foreach (var item in temp)
            {
                CollectionItem i = new CollectionItem();
                i.Item = item;
                Emojit.Add(i);
            }
            Console.WriteLine("emojeita listassa lisätiedothost.cs:ssä: " + s.emojis.Count);
            int count = 0;
            double calculateScale = 0.0;
            Dictionary<int, int> sorted = new Dictionary<int, int>();
            foreach (KeyValuePair<int, int> item in Main.GetInstance().host.data.GetEmojiResults().OrderByDescending(key => key.Value))
            {
                sorted.Add(item.Key, item.Value);
                resultAmount.Add(item.Value);
                count += item.Value;
            }
            foreach (int key in sorted.Keys)
            {
                resultImages.Add("emoji" + key.ToString() + "lowres.png");
                Console.WriteLine(key.ToString() + "ja resultimagesissa nyt: " + resultImages.Count);
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
        async void KeskeytäClicked(object sender, EventArgs e)
        {
            //Sulkee kyselyn kaikilta osallisujilta (linjat poikki höhö XD)


            var res = await DisplayAlert("Haluatko varmasti sulkea huoneen?", "", "Kyllä", "Ei");

            if (res == true)
            {
                await Navigation.PopToRootAsync();
            }
            else return;

        }
        async void JatkaClicked(object sender, EventArgs e)
        {
            //Siirrytään odottamaan äänestyksen tuloksia (HOST)
            await Navigation.PushAsync(new TulostenOdotus());

            //Hox, Clientin pitää päästä vastaamaan kyselyy, ei hostin
        }
    }
}