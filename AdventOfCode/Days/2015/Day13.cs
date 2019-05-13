using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions;

namespace AdventOfCode.Days._2015
{
    // ReSharper disable once UnusedMember.Global
    public class Day13 : Day
    {
        public Day13() : base(2015, 13) { }

        private Dictionary<string, int> neighbourStats = new Dictionary<string, int>();
        private HashSet<string> guests = new HashSet<string>();
        private Regex regLine = new Regex(@"(\w+) .+ (gain|lose) (\d+) .+ (\w+)\.");
        
        private void ParseLine(Dictionary<string, int> dict, HashSet<string>guestList, string line)
        {
            var groups = regLine.Match(line).Groups;
            string combo = groups[1].Value + groups[4].Value;
            int happiness = groups[3].Value.ToInt();
            if (groups[2].Value == "lose")
            {
                happiness *= -1;
            }
            guestList.Add(groups[1].Value);
            dict.Add(combo, happiness);
        }

        private int CalculateHappiness(List<string> people)
        {
            int sum = 0;
            for (int i = 0; i < people.Count; i++)
            {
                int l = i - 1 < 0 ? people.Count - 1 : i - 1;
                int r = i + 1 >= people.Count ? 0 : i + 1;

                sum += neighbourStats[people[i] + people[l]];
                sum += neighbourStats[people[i] + people[r]];
            }
            return sum;
        }

        private int GetHappiness(List<string> people, List<string> guestsRemaining )
        {
            if (guestsRemaining.Count > 0)
            {
                List<int> results = new List<int>();
                foreach (string s in guestsRemaining)
                {
                    List<string> p = new List<string>(people) {s};
                    results.Add(GetHappiness(p, guestsRemaining.Where(g => g != s).ToList()));
                }
                return results.Max();
            }
            else
            {
                return CalculateHappiness(people);
            }
            
        }
        
        protected override object GetSolutionPart1()
        {
            foreach (string line in InputLines)
            {
                ParseLine(neighbourStats, guests, line);
            }

            List<int> results = new List<int>();

            foreach (string guest in guests)
            {
                results.Add(GetHappiness(new List<string>() {guest}, guests.Where(g => g != guest).ToList()));
            }
            
            return results.Max().ToString();
        }

        protected override object GetSolutionPart2()
        {
            foreach (string guest in guests)
            {
                neighbourStats.Add(guest + "Me", 0);
                neighbourStats.Add("Me" + guest, 0);
            }

            List<int> results = new List<int>();
            guests.Add("Me");
            foreach (string guest in guests)
            {
                results.Add(GetHappiness(new List<string>() { guest }, guests.Where(g => g != guest).ToList()));
            }

            return results.Max().ToString();
        }
    }
}
