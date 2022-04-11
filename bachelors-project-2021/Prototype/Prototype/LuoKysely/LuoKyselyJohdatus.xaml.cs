
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
    public partial class LuoKyselyJohdatus : ContentPage
    {

        public IList<string> introMessage { get; set; }
        public string selectedItem = null;



        public LuoKyselyJohdatus()
        {
            InitializeComponent();

            introMessage = Const.intros;



            if(Main.GetInstance().GetMainState() == Main.MainState.Editing)
            {
                selectedItem = SurveyManager.GetInstance().GetSurvey().introMessage;
                //JButton.Text = selectedItem;
                //JatkaBtn.IsEnabled = true;
            }
           

       
            BindingContext = this;


 
        }


        async void OnPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;
            
            if (selectedIndex != -1) {
          /*      if(selectedIndex == 2)
                {
                    await Navigation.PushAsync(new Omakysymys());
                }*/
                //ottaa talteen kysymyksen
                selectedItem = KysymysPicker.Items[KysymysPicker.SelectedIndex];
                JatkaBtn.IsEnabled = true;
            }

            else {
                JatkaBtn.IsEnabled = false;
            }
        }



        async void JatkaButtonClicked(object sender, EventArgs e)
        {
            //tallentaa kyselyn kysymyksen
            SurveyManager.GetInstance().GetSurvey().introMessage = selectedItem;
          

            //siirrytään emojin valinta sivulle 
            await Navigation.PushAsync(new LuoKyselyEmojit());
        }

        async void EdellinenButtonClicked(object sender, EventArgs e) 
        {
            //survey resetoidaan
            SurveyManager.GetInstance().ResetSurvey();

            await Navigation.PushAsync(new Opettajanhuone());
        }
    }

  
}