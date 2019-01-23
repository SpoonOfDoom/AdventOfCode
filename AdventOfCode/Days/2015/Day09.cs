using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions;

namespace AdventOfCode.Days._2015
{
    // ReSharper disable once UnusedMember.Global
    public class Day09 : Day
    {
        public Day09() : base(2015, 9) { }

        struct Route
        {
            public string City1;
            public string City2;
            public int Distance;
        }

        private List<Route> routes = new List<Route>();
        private HashSet<string> cities = new HashSet<string>();

        private int GetDistance(string start, int distance, List<string> targets, bool longest = false)
        {
            if (targets.Count == 1)
            {
                return distance + routes
                    .Where(r => r.City1 == start && r.City2 == targets.Single() || r.City2 == start && r.City1 == targets.Single())
                    .Select(r => r.Distance)
                    .Single();
            }

            List<int> distances = new List<int>();

            foreach (var target in targets)
            {
                distances.Add(
                    GetDistance(
                        target,
                        routes.Where(r => r.City1 == start && r.City2 == target || r.City2 == start && r.City1 == target).Select(r => r.Distance).Single(),
                        targets.Where(t => t != target).ToList(),
                        longest
                        )
                    );
            }
            if (longest)
            {
                return distance + distances.Max();
            }
            else
            {
                return distance + distances.Min();
            }
            
        }

        protected override object GetSolutionPart1()
        {
            Regex regLine = new Regex(@"(\w+) to (\w+) = (\d+)");
            

            foreach (string line in InputLines)
            {
                var groups = regLine.Match(line).Groups;
                Route r = new Route
                {
                    City1 = groups[1].Value,
                    City2 = groups[2].Value,
                    Distance = groups[3].Value.ToInt()
                };
                cities.Add(groups[1].Value);
                cities.Add(groups[2].Value);
                routes.Add(r);
            }
            
            List<int> distances = new List<int>();
            foreach (var start in cities)
            {
                distances.Add(GetDistance(start, 0, cities.Where(t => t != start).ToList()));
            }
            return distances.Min().ToString();
        }

        protected override object GetSolutionPart2()
        {
            List<int> distances = new List<int>();
            foreach (var start in cities)
            {
                distances.Add(GetDistance(start, 0, cities.Where(t => t != start).ToList(), true));
            }
            return distances.Max().ToString();
        }
    }
}
