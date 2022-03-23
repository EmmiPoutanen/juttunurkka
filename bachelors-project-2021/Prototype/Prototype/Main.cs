
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
using System.Threading.Tasks;

namespace Prototype
{	
	//Singleton Main class
	public class Main
	{
		private static Main instance = null;
		public enum MainState
		{
			Default = 0,
			Browsing = 1,
			Editing = 2,
			Hosting = 3,
			Participating = 4,
			CreatingNew = 5
		}
		public MainState state;
		public SurveyClient client = null;
		public SurveyHost host = null;
		
		private Main() {
			state = MainState.Default;
		}
		
		public void BrowseSurveys() {
			Console.WriteLine($"DEBUG: Browsing surveys");
			state = MainState.Browsing;
		}

		public void EditSurvey() {
			Console.WriteLine($"DEBUG: Editing selected survey");
			state = MainState.Editing;

		}
		public void CreateNewSurvey()
		{
			Console.WriteLine("Creating new survey");
			state = MainState.CreatingNew;
			SurveyManager.GetInstance().ResetSurvey();
		}

		public async Task<bool> JoinSurvey(string RoomCode) {
			Console.WriteLine($"DEBUG: Attempting new client instance with RoomCode: {RoomCode}");
			client = new SurveyClient();
			bool success = await Task.Run(() => client.LookForHost(RoomCode));
			if (success) {
				state = MainState.Participating;
			}
			return success;
		}	

		public async Task<bool> HostSurvey() {
			Console.WriteLine($"DEBUG: Creating new host instance with selected survey");
			state = MainState.Hosting;
			host = new SurveyHost();
			return await host.RunSurvey();
		}

		public MainState GetMainState() {
			return state;
		}
		
		//Singleton instance getter
		public static Main GetInstance() {
			if (instance != null) {
				return instance;
			}
			instance = new Main();
			return instance;
		}
	}
}
