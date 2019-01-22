using System;
using AdventOfCode.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Days
{
    // ReSharper disable once UnusedMember.Global
    public class Day17 : Day
    {
        public Day17() : base(17) { }

        private List<int> containers;
        private const int target = 150;

        private List<List<int>> GetIndexCombinations(List<int> containers, bool minimum = false)
        {
            List<List<int>> retLists = new List<List<int>>();

            for (int i = 1; i <= containers.Count; i++)
            {
                Console.WriteLine(i + "...");
                var l = containers.DifferentCombinations(i).Select(e => e.ToList());
                retLists.AddRange(l);
                if (minimum && l.Any(el => el.Sum() == 150))
                {
                    break;
                }
            }
            return retLists;
        }

        protected override object GetSolutionPart1()
        {
            containers = InputLines.Select(i => i.ToInt()).ToList();
            
            var results = GetIndexCombinations(containers).Where(element => element.Sum() == 150).ToList();
            return results.Count.ToString();
        }

        protected override object GetSolutionPart2()
        {
            var results = GetIndexCombinations(containers, true).Where(element => element.Sum() == 150).ToList();

            return results.Count.ToString();
        }
    }
}
