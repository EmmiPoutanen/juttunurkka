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
using Microsoft.Maui.Controls;

namespace Prototype
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Omakysymys : ContentPage
    {
        private string CustomQuestion = ""; // Tallennetaan käyttäjän syöttämä kysymys

        public Omakysymys()
        {
            InitializeComponent();
        }

        // Käsittelee käyttäjän kirjoittaman kysymyksen ja päivittää Jatka-painikkeen tilan
        private void OnCustomQuestionEntered(object sender, TextChangedEventArgs e)
        {
            CustomQuestion = e.NewTextValue?.Trim(); // Tallennetaan käyttäjän syöttämä teksti
            ((Button)FindByName("JatkaButton")).IsEnabled = !string.IsNullOrWhiteSpace(CustomQuestion);
        }

        // Jatka-painikkeen toiminnallisuus
        async void JatkaButtonClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CustomQuestion))
            {
                await DisplayAlert("Huomio!", "Kirjoita ensin kysymys.", "OK");
                return;
            }

            // Tallennetaan kysymys SurveyManageriin
            SurveyManager.GetInstance().GetSurvey().introMessage = CustomQuestion;

            // Siirrytään seuraavalle sivulle
            await Navigation.PushAsync(new LuoKyselyEmojit());
        }

        // Edellinen-painikkeen toiminnallisuus
        async void EdellinenButtonClicked(object sender, EventArgs e)
        {
            // Nollataan kysely
            SurveyManager.GetInstance().ResetSurvey();

            // Jos ollaan editointitilassa, palataan tarkastelusivulle, muuten etusivulle
            if (Main.GetInstance().GetMainState() == Main.MainState.Editing)
            {
                Main.GetInstance().BrowseSurveys();
                await Navigation.PopAsync();
            }
            else
            {
                await Navigation.PopToRootAsync();
            }
        }
    }
}
