﻿/*
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

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Prototype
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Kirjautuminen : ContentPage
    {
        public Kirjautuminen()
        {

            InitializeComponent();
        }

        private void MyClick(object sender, EventArgs e)
        {
            if(txtUsername.Text =="opettaja" && txtPassword.Text == "opehuone")
            { 
            
            }
            else
            {
                DisplayAlert("VIRHE", "Vaara salasana tai kayttajatunnus", "OK");
            }
        }

        async void KirjauduClicked(object sender, EventArgs e)
        {
            if (txtUsername.Text == "opettaja" && txtPassword.Text == "opehuone")
            {
                await Navigation.PushAsync(new Opettajanhuone());
            }
            else
            {
                await DisplayAlert("VIRHE", "Vaara salasana tai kayttajatunnus", "OK");
            }
            
        }
    }
}