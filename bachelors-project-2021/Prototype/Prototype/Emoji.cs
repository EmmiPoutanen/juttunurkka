
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
	public class Emoji
	{
		public int ID {get; set;} = 0;
		public string Name { get; set; } = "default";
		public string Impact { get; set; } = "default";
		public IList<string> activities { get; set; } = null;
		public string ImageSource { get; set; } = "missing.png";

		public Emoji()
		{
			activities = new List<string>() { "foo", "bar" };
		}

		public Emoji(int ID, string Name, string impact, List<string> activities, string ImageSource)
		{
			this.ID = ID;
			this.Name = Name;
			this.Impact = impact;
			this.activities = activities;
			this.ImageSource = ImageSource;
		}
		 
		public override string ToString()
		{
			string value = "";

			value += $"ID: {ID}, ";
			value += $"Name: {Name}, ";
			value += $"Impact: {Impact}, ";

			value += "Activities: [";			
			foreach (var item in activities)
			{
				value += $"{item} ";
			}
			value += "], ";

			value += $"ImageSource: {ImageSource}";

			return value;
		}
	}
}
 
