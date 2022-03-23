
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

using System.Collections.Generic;
using System;

namespace Prototype
{
	public class Survey
	{
		public string introMessage { get; set; }
		public List<Emoji> emojis { get; set; }
		public string RoomCode { get; set; }
		public string Name { get; set; }
		
		//empty constructor creates blank survey
		public Survey() {
			introMessage = null;
			emojis = new List<Emoji>();

			emojis.Add(new Emoji(0, "Iloinen", "neutral", new List<string>(), "emoji0lowres.png"));
			emojis.Add(new Emoji(1, "Hämmästynyt", "neutral", new List<string>(), "emoji1lowres.png"));
			emojis.Add(new Emoji(2, "Neutraali", "neutral", new List<string>(), "emoji2lowres.png"));
			emojis.Add(new Emoji(3, "Vihainen", "neutral", new List<string>(), "emoji3lowres.png"));
			emojis.Add(new Emoji(4, "Väsynyt", "neutral", new List<string>(), "emoji4lowres.png"));
			emojis.Add(new Emoji(5, "Miettivä", "neutral", new List<string>(), "emoji5lowres.png"));
			emojis.Add(new Emoji(6, "Itkunauru", "neutral", new List<string>(), "emoji6lowres.png"));

			RoomCode = null;
			Name = null;
		}
		public Survey(string introMessage, List<Emoji> emojis, string RoomCode, string Name)
		{
			this.introMessage = introMessage;
			this.emojis = emojis;
			this.RoomCode = RoomCode;
			this.Name = Name;
		}

		//default survey consists of first intro entry, 7 emojis with various impact each with 3 first entries of activities and a random roomcode
		public static Survey GetDefaultSurvey()
		{
			string tempIntro = Const.intros[0];
			List<Emoji> tempEmojis = new List<Emoji>();

			List<string> activities;

			Const.activities.TryGetValue(0, out activities);
			activities = activities.GetRange(0, 2);
			tempEmojis.Add(new Emoji(0, "Iloinen", "positive", activities, "emoji0lowres.png"));

			Const.activities.TryGetValue(1, out activities);
			activities = activities.GetRange(0, 2);
			tempEmojis.Add(new Emoji(1, "Hämmästynyt", "negative", activities, "emoji1lowres.png"));

			Const.activities.TryGetValue(2, out activities);
			activities = activities.GetRange(0, 3);
			tempEmojis.Add(new Emoji(2, "Neutraali", "neutral", activities, "emoji2lowres.png"));

			Const.activities.TryGetValue(3, out activities);
			activities = activities.GetRange(0, 2);
			tempEmojis.Add(new Emoji(3, "Vihainen", "negative", activities, "emoji3lowres.png"));

			Const.activities.TryGetValue(4, out activities);
			activities = activities.GetRange(0, 2);
			tempEmojis.Add(new Emoji(4, "Väsynyt", "neutral", activities, "emoji4lowres.png"));

			Const.activities.TryGetValue(5, out activities);
			activities = activities.GetRange(0, 3);
			tempEmojis.Add(new Emoji(5, "Miettivä", "neutral", activities, "emoji5lowres.png"));

			Const.activities.TryGetValue(6, out activities);
			activities = activities.GetRange(0, 3);
			tempEmojis.Add(new Emoji(6, "Itkunauru", "positive", activities, "emoji6lowres.png"));

			string TempRoomCode = GenerateRandomCode();

			return new Survey(tempIntro, tempEmojis, TempRoomCode, "Oletus");
		}

		//generates random room code of 5 numbers
		private static string GenerateRandomCode()
		{
			Random r = new Random();
			string temp = "";

			for (int i = 0; i < 5; i++)
			{
				temp += r.Next(10).ToString();
			}

			return temp;
		}
		//toString method for getting the info of the survey in a string for data purposes
		public override string ToString() {
			string value = "";

			value += $"Intro: {introMessage}\n";

			foreach (var item in emojis)
			{
				value += item.ToString();
				value += "\n";
			}

			value += $"RoomCode: {RoomCode}";

			return value;
		}
	}
}
