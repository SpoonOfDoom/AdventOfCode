using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdventOfCode.Extensions;
using AdventOfCode2016.Tools;

namespace AdventOfCode.Days
{
    public class Day24 : Day
    {
        public Day24() : base(24) { }

        List<int> allPresents = new List<int>();
        int totalWeight;
        
        
        class IntListEqualityComparer : IEqualityComparer<List<int>>
        {
            public bool Equals(List<int> x, List<int> y)
            {
                if (x.Count != y.Count)
                {
                    return false;
                }
                for (int i = 0; i < x.Count; i++)
                {
                    if (x[i] != y[i])
                    {
                        return false;
                    }
                }
                return true;
            }

            public int GetHashCode(List<int> obj)
            {
                int hash = 421337;
                foreach (int i in obj)
                {
                    hash = hash * i.GetHashCode();
                }
                return hash;
            }
        }

        private HashSet<List<int>> MakeTriplets(List<int> presents, int parts = 3)
        {
            HashSet<List<int>> combinations = new HashSet<List<int>>(new IntListEqualityComparer());
            Moep(combinations, new List<int>(), presents, 1, parts);
            
            return combinations;
        }


        private void Moep(HashSet<List<int>> combinations,  List<int> combo, List<int> presents, int level, int parts = 3)
        {
            if (combo.Sum() == totalWeight / parts)
            {
                combo.Sort();
                combinations.Add(combo);
            }
            if (presents.Count == 0)
            {
                return;
            }
            if (combo.Sum() > totalWeight / parts)
            {
                return;
            }
            else
            {
                foreach (int present in presents)
                {
                    List<int> newCombo = new List<int>(combo);
                    newCombo.Add(present);
                    int index = presents.IndexOf(present);
                    Moep(combinations, newCombo, presents.GetRange(index+1, presents.Count -(index+1)), level + 1, parts);
                    
                }
            }
        }
        
        public override string GetSolutionPart1()
        {
            allPresents.AddRange(inputLines.Select(line => line.ToInt()));
            allPresents.Reverse();
            totalWeight = allPresents.Sum();
            HashSet<List<int>> possiblePresentBags = MakeTriplets(allPresents);

            int smallestCount = possiblePresentBags.Min(bag => bag.Count);
            var combinations = possiblePresentBags.Where(b => b.Count == smallestCount);
            long best = long.MaxValue;
            foreach (List<int> combination in combinations)
            {
                long entanglement = 1;
                foreach (int present in combination)
                {
                    entanglement *= present;
                }
                if (entanglement < best)
                {
                    best = entanglement;
                }
            }
            long result = best;
            return result.ToString(); //11266889531
        }

        public override string GetSolutionPart2()
        {
            HashSet<List<int>> possiblePresentBags = MakeTriplets(allPresents, 4);

            int smallestCount = possiblePresentBags.Min(bag => bag.Count);
            var combinations = possiblePresentBags.Where(b => b.Count == smallestCount);
            long best = long.MaxValue;
            foreach (List<int> combination in combinations)
            {
                long entanglement = 1;
                foreach (int present in combination)
                {
                    entanglement *= present;
                }
                if (entanglement < best)
                {
                    best = entanglement;
                }
            }
            long result = best;
            return result.ToString(); //77387711
        }
    }
}