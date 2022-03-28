
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
    public partial class LuoKyselyEmojit : ContentPage
    {

        public IList<CollectionItem> Emojit { get; private set; }
        
        public class CollectionItem {
            public Emoji Item { get; set; } = null;
            public bool CheckBox { get; set; } = false;
            public bool IsNegative { get; set; } = false;
            public bool IsNeutral { get; set; } = false;
            public bool IsPositive { get; set; } = false;
		}

       

        public LuoKyselyEmojit()
        {
            InitializeComponent();

            //alustetaan emojit kyselyn emojeilla
            Emojit = new List<CollectionItem>();

            List<Emoji> temp = SurveyManager.GetInstance().GetSurvey().emojis;

			//alustetaan radionappien valinnat
            //ei saa kyseenalaistaa tätä toteutusta, radionappeihin ei oikeastaan pääse käsiksi collection view layoutin sisältä
			foreach (var item in temp)
			{
                CollectionItem i = new CollectionItem();
				i.Item = item;

				switch (item.Impact)
				{
					case "positive":
                        i.IsPositive = true;
						break;
                    case "neutral":
                        i.IsNeutral =true;
                        break;
                    case "negative":
                        i.IsNegative = true;
                        break;
                }
                i.CheckBox = false;
                Emojit.Add(i);
			}

            BindingContext = this;
        }

        void OnCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            List<Emoji> tempEmojis= new List<Emoji>();
            List<string> tempNames = new List<string>();

            if (sender is CheckBox c && c.Parent is Grid g)
            {
                c.Color=Color.Pink;
        // testing oikea emoji,voi poistaa PP        g.Children[0].IsVisible=false;
                if (c.IsChecked == true)
                {
                    Console.WriteLine("on");
                //    tempEmojis.Add();
                }
                else
                {

                }
            }
            else
            {
                //doSmthng
            }
            JatkaBtn.IsEnabled = true;
        }

        void OnAllCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e)
        {

        }


        async void EdellinenButtonClicked(object sender, EventArgs e) 
        {
            await Navigation.PushAsync(new LuoKyselyJohdatus());
        }
    async void JatkaButtonClicked(object sender, EventArgs e)
        {
            //asetetaan emojit survey olioon
            List<Emoji> temp = new List<Emoji>();
            foreach (var item in Emojit)
			{
                if (item.IsPositive) {
                    item.Item.Impact = "positive";
				} else if (item.IsNeutral) {
                    item.Item.Impact = "neutral";
                } else if (item.IsNegative)
                    item.Item.Impact = "negative";
                if (item.CheckBox ==true )
                {
                    Console.WriteLine("on tosi");
                    item.Item.IsChecked = true;
                    // vain jos emojiin liittyvä checkbox on chekattu niin lisätään se emojilistaan.PP
                    temp.Add(item.Item);
                    Console.WriteLine(item.Item.Name);
                }
                else
                {
                    //
                }
 
                //entistä koodia
               // temp.Add(item.Item);

            }
            //katsotaan mikä on listan pituus konsolilta PP
            Console.WriteLine(temp.Count);
            SurveyManager.GetInstance().GetSurvey().emojis = temp;
            

            // siirrytään aktiviteetit sivulle 
            await Navigation.PushAsync(new LuoKyselyToimenpiteet()); 
        }
    }
}