
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

		public static Dictionary<int, List<string>> activities = new Dictionary<int, List<string>>() {
			{ 0, new List<string>() {
				"Jokainen kertoo mikä on kivaa",
				"Kehu vieressä istuvaa",
				"Valitaan päivän apuope",
			}},
			{ 1, new List<string>() {
				"Kerrotaan ohjaajalle mikä hämmästyttää",
				"Kerrotaan ryhmälle mikä hämmästyttää",
			}},
			{ 2, new List<string>() {
				"Piirretään taululle",
				"Jokainen kertoo opettajalle yhden mietteen",
				"Jokainen kertoo ryhmälle yhden ajatuksen",
			}},
			{ 3, new List<string>() {
				"Positiivinen palloleikki",
				"Kirjoitetaan ohjaajalle salattu lappu",
			}},
			{ 4, new List<string>() {
				"5 minuutin tauko",
				"Siirretään oppitunti ulos",
			}},
			{ 5, new List<string>() {
				"Jokainen kertoo yhden asian mikä mietityttää",
				"Jokainen kysyy kysymyksen ohjaajalta",
				"Jokainen kysyy kysymyksen ryhmältä",
			}},
			{ 6, new List<string>() {
				"Naurujoogatuokio",
				"Katsotaan video",
				"Kerrotaan vitsi",
			}}
		};
	
		public static class Network {
			public static int ServerUDPClientPort = 43256;
			public static int ServerTCPListenerPort = 43257;
			public static int ClientUDPClientPort = 43258;
		}
	}
}
