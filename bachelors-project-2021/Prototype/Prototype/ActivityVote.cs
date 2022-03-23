
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

namespace Prototype
{
    public class ActivityVote
    {
        private Dictionary<int, IList<string>> vote1Candidates;
        private List<string> vote2Candidates;
        private readonly int totalCount = Main.GetInstance().host.data.totalEmojis;
        private string finalResult;
        public int vote1Timer = 0;
        public int vote2Timer = Const.vote2Time;
        public int coolDown = 5;


        public ActivityVote ()
        {
            vote1Candidates = new Dictionary<int, IList<string>>();
            vote2Candidates = new List<string>();
        }

        public void calcVote1Candidates(List<Emoji> emojis, Dictionary<int, int> emojiResults)
        {
            //Creating dictionary for threat ranking and sorted threat ranking
            Dictionary<int, double> ranking = new Dictionary<int, double>();
            Dictionary<int, double> sortedRanking = new Dictionary<int, double>();
            double percentage = 0.0;
            double tolerance = 0.0;
            double threat = 0.0;

            foreach (KeyValuePair<int, int> answer in emojiResults)
            {
                percentage = (double)answer.Value / totalCount;
                
                //assigning tolerance for each answer in emojiResults
                if (emojis[answer.Key].Impact == "negative")
                {
                    tolerance = 0;
                }
                if (emojis[answer.Key].Impact == "neutral")
                {
                    tolerance = 0.25;
                }
                if (emojis[answer.Key].Impact == "positive")
                {
                    tolerance = 0.5;
                }

                //Calculation for the threat value
                threat = percentage - tolerance;
                Console.WriteLine("key: {0}, percentage: {1}, threat: {2}", answer.Key, percentage, threat);

                //Adding each answer to the ranking dictionary
                ranking.Add(answer.Key, threat);
            }

            //Sorting each item in ranking by threat value (highest threat is at top)
            foreach (KeyValuePair<int, double> item in ranking.OrderByDescending(key => key.Value))
            {
                sortedRanking.Add(item.Key, item.Value);
            }
            
            //if top ranking threat is higher than 0 a.k.a. threat is above threshold we add it in the vote1candidates if the threat value is above 0
            if(sortedRanking.Values.ElementAt(0) > 0)
            {
                foreach (KeyValuePair<int, double> item in sortedRanking)
                {
                    if (item.Value > 0)
                    {
                        vote1Candidates.Add(item.Key, emojis[item.Key].activities);
                    }
                }
            }
            // if threat value us 0 or lower add the top 2 to the vote1candidates, top 2 works since the ranking list is practically all emojis
            if(sortedRanking.Values.ElementAt(0) <= 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    vote1Candidates.Add(sortedRanking.Keys.ElementAt(i), emojis[sortedRanking.Keys.ElementAt(i)].activities);
                }
            }
            vote1Timer = (Const.vote1PerEmojiTime * vote1Candidates.Count) + 10;
        }
        
        public void calcVote2Candidates(Dictionary<(int, string), int> vote1Results)
        {
            //fallback, if nobody votes in phase 1
            if (vote1Results.Count == 0)
            {
                Console.WriteLine("We did not receive any votes in phase 1, default fallback = first activity in activities of each emoji");
				foreach (var item in vote1Candidates)
				{
                    vote2Candidates.Add(item.Value.ElementAt(0));
				}
            }

            //for getting a sorted list out of vote1Results
            Dictionary<(int, string), int> sorted = new Dictionary<(int, string), int>();
            foreach (KeyValuePair<(int, string), int> item in vote1Results.OrderByDescending(key => key.Value))
            {
                Console.WriteLine("key: {0}, value: {1}", item.Key, item.Value);
                sorted.Add(item.Key, item.Value);
            }

            //temporary list for parsing the sorted list from duplicate IDs leaving the most voted activity per emoji
            Dictionary<int, string> tempList = new Dictionary<int, string>();
            foreach(var key in sorted.Keys)
            {
                if (!tempList.ContainsKey(key.Item1))
                {
                    Console.WriteLine("keyItem1: {0}, KeyItem2: {1}", key.Item1, key.Item2);
                    tempList.Add(key.Item1, key.Item2);
                }
            }

            //adding each sorted key (previously voted activities) to vote2Candidates list
            foreach (var item in tempList.Values)
            {
                vote2Candidates.Add(item);
            }
        }

        //get vote1candidates
        public Dictionary<int, IList<string>> GetVote1Candidates() {
            return vote1Candidates;
		}
        //set vote1candidates
        public void SetVote1Candidates(Dictionary<int, IList<string>> candidates) {
            vote1Candidates = candidates;
		}

        //get vote2candidates
        public List<string> GetVote2Candidates() {
            return vote2Candidates;
        }
        //set vote2candidates
        public void SetVote2Candidates(List<string> candidates)
        {
            vote2Candidates = candidates;
        }

        public string calcFinalResult(Dictionary<string, int> vote2Results)
        {
            //fallback, if nobody voted in phase 2
			if (vote2Results.Count == 0)
			{
                Console.WriteLine("We did not receive any votes in phase 2, default fallback = activity of the emoji with highest level of concern");
                return vote2Candidates.ElementAt(0);
			}

            //creating empty dictionary sorted
            Dictionary<string, int> sorted = new Dictionary<string, int>();
            foreach (KeyValuePair<string, int> item in vote2Results.OrderByDescending(key => key.Value))
            {
                sorted.Add(item.Key, item.Value);
            }

            //final result is the top from sorted list of vote2results
            finalResult = sorted.Keys.ElementAt(0);

            return finalResult;
        }

        public override string ToString()
        {
            string value = "";

            
            foreach (var item in vote1Candidates)
            {
                value += $"ID: {item.Key.ToString()}, ";
                value += "Activities: [";
                foreach (var activity in item.Value)
                {
                    value += $"{activity} ";
                }
                value += "]";
                value += "\n";
            }
            

            foreach(var item in vote2Candidates)
            {
                value += $"Activity: {item}";
                value += "\n";
            }
            

            value += finalResult;
            
            return value;
        }
    }
}
