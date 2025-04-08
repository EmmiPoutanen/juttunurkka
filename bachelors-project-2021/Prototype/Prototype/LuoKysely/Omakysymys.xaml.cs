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

namespace Prototype
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Omakysymys : ContentPage
    {
        public Omakysymys()
        {
            InitializeComponent();

            BindingContext = this;
        }

        async void JatkaButtonClicked(object sender, EventArgs e)
        {
            //kyselyn johdatuslause asetetaan.
            //SurveyManager.GetInstance().GetSurvey().introMessage = selectedItem;


            //siirrytään "luo uus kysely 2/3" sivulle 

            string omaKysymys = Kysymys.Text;
            SurveyManager.GetInstance().GetSurvey().introMessage = omaKysymys;

            await Navigation.PushAsync(new LuoKyselyEmojit());
        }

        async void EdellinenButtonClicked(object sender, EventArgs e) //menee vielä kokonaan alkuun
        {
            //survey resetoidaan
            SurveyManager.GetInstance().ResetSurvey();

            //Jos ollaan edit tilassa, niin siirrytään takaisin kyselyntarkastelu sivulle, muutoin main menuun
            if (Main.GetInstance().GetMainState() == Main.MainState.Editing)
            {
                Main.GetInstance().BrowseSurveys();
                await Navigation.PopAsync();
            }
            else
            {
                // siirrytään etusivulle
                await Navigation.PopToRootAsync();
            }
        }
    }
}