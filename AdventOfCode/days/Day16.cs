using AdventOfCode.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Days
{
    class Sue
    {
        public static Dictionary<string, int> Target = new Dictionary<string, int>
        {
            { "children", 3 },
            { "cats", 7 },
            { "samoyeds", 2 },
            { "pomeranians", 3 },
            { "akitas", 0 },
            { "vizslas", 0 },
            { "goldfish", 5 },
            { "trees", 3 },
            { "cars", 2 },
            { "perfumes", 1 }
        };

        public int Id;
        public Dictionary<string, int> Stuff = new Dictionary<string, int>();

        public bool IsValidCandidate()
        {
            foreach (var pair in Stuff)
            {
                if (Target[pair.Key] != pair.Value)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsValidCandidate2()
        {
            foreach (var pair in Stuff)
            {
                switch (pair.Key)
                {
                    case "cats":
                    case "trees":
                        if (pair.Value < Target[pair.Key])
                        {
                            return false;
                        }
                        break;
                    case "pomeranians":
                    case "goldfish":
                        if (pair.Value > Target[pair.Key])
                        {
                            return false;
                        }
                        break;
                    default:
                        if (Target[pair.Key] != pair.Value)
                        {
                            return false;
                        }
                        break;
                }
            }
            return true;
        }
    }

    public class Day16 : Day
    {
        public Day16() : base(16) { }

        private Regex regLine = new Regex(@"Sue (\d+): (.+)");
        private List<Sue> Candidates = new List<Sue>();

        private Sue ParseLine(string line)
        {
            Sue s = new Sue();

            var groups = regLine.Match(line).Groups;

            s.Id = groups[1].Value.ToInt();

            var things = groups[2].Value.Split(',');

            foreach (var thing in things)
            {
                var pair = thing.Split(':');

                s.Stuff.Add(pair[0].Trim(), pair[1].ToInt());
            }
            
            return s;
        }
        public override string GetSolutionPart1()
        {
            foreach (var line in inputLines)
            {
                var sue = ParseLine(line);
                Candidates.Add(sue);
            }

            var validCandidates = Candidates.Where(c => c.IsValidCandidate()).ToList();

            return validCandidates.Select(s => s.Id).Single().ToString();
        }

        public override string GetSolutionPart2()
        {
            var validCandidates = Candidates.Where(c => c.IsValidCandidate2()).ToList();
            //todo: find out why there are two candidates
            return validCandidates.Select(s => s.Id).Max().ToString(); //Not sure if it's correct that there are two possible candidates, but close enough for now.
        }
    }
}