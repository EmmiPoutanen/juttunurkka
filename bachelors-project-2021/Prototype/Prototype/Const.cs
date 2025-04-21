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
using System.Text;

namespace Prototype
{
	public static class Const
	{
		public static int vote1PerEmojiTime = 10;
		public static int vote2Time = 10;

		public static List<string> intros = new List<string>() {
			"Tänään minusta tuntuu",
			"Minun päiväni on ollut",
			"Tämä tuntui mielestäni",
			"Mitä mieltä olit tunnista?",
			"Tämä tunti oli mielestäni"
		};

		public static Dictionary<int, List<Activity>> activities = new Dictionary<int, List<Activity>>() {
            { 0, new List<Activity>()
			  {
				  new() { Title = "Jokainen kertoo mikä on kivaa", ImageSource = "thumbsUp.png" },
				  new() { Title = "Kehu vieressä istuvaa", ImageSource = "thumbsUp.png" },
				  new() { Title = "Valitaan päivän apuope", ImageSource = "thumbsUp.png" }
			  }
			},
            { 1, new List<Activity>() {
                new() { Title = "Kerrotaan ohjaajalle mikä hämmästyttää", ImageSource = "thumbsUp.png" },
                new() { Title = "Kerrotaan ryhmälle mikä hämmästyttää", ImageSource = "thumbsUp.png" },
            }},
			{ 2, new List<Activity>() {
                new() { Title = "Piirretään taululle", ImageSource = "thumbsUp.png" },
                new() { Title = "Jokainen kertoo opettajalle yhden mietteen", ImageSource = "thumbsUp.png" },
                new() { Title = "Jokainen kertoo ryhmälle yhden ajatuksen", ImageSource = "thumbsUp.png" },
            }},
			{ 3, new List<Activity>() {
                new() { Title = "Positiivinen palloleikki", ImageSource = "thumbsUp.png" },
                new() { Title = "Kirjoitetaan ohjaajalle salattu lappu", ImageSource = "thumbsUp.png" },
            }},
			{ 4, new List<Activity>() {
                new() { Title = "5 minuutin tauko", ImageSource = "thumbsUp.png" },
                new() { Title = "Siirretään oppitunti ulos", ImageSource = "thumbsUp.png" },
            }},
			{ 5, new List<Activity>() {
                new() { Title = "Jokainen kertoo yhden asian mikä mietityttää", ImageSource = "thumbsUp.png" },
                new() { Title = "Jokainen kysyy kysymyksen ohjaajalta", ImageSource = "thumbsUp.png" },
                new() { Title = "Jokainen kysyy kysymyksen ryhmältä", ImageSource = "thumbsUp.png" },
            }},
			{ 6, new List<Activity>() {
                new() { Title = "Naurujoogatuokio", ImageSource = "thumbsUp.png" },
                new() { Title = "Katsotaan video", ImageSource = "thumbsUp.png" },
                new() { Title = "Kerrotaan vitsi", ImageSource = "thumbsUp.png" },
            }}
		};
	
		public static class Network {
			public static int ServerUDPClientPort = 43256;
			public static int ServerTCPListenerPort = 43257;
			public static int ClientUDPClientPort = 43258;
		}
	}
}
