using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Extensions;
using AdventOfCode2016.Tools;

namespace AdventOfCode.Days
{
    public class Day24 : Day
    {
        public Day24() : base(24) { }

        List<int> allPresents = new List<int>();
        int totalWeight;

        class PresentGroup
        {
            public List<int> Presents = new List<int>();
            int? quantumEntanglement;
            public int Count => Presents.Count;
            public int Weight => Presents.Count == 0 ? 0 : Presents.Sum();

            public int QuantumEntanglement
            {
                get
                {
                    if (quantumEntanglement.HasValue)
                    {
                        return quantumEntanglement.Value;
                    }
                    quantumEntanglement = CalculateQuantumEntanglement();
                    return quantumEntanglement.Value;
                }
            }

            public bool Equals(PresentGroup otherGroup)
            {
                if (Count != otherGroup.Count)
                {
                    return false;
                }

                return QuantumEntanglement == otherGroup.QuantumEntanglement;
            }

            public void AddPresent(int present)
            {
                if (Presents.Contains(present))
                {
                    throw new Exception("Duplicate present!");
                }
                Presents.Add(present);
            }

            private int CalculateQuantumEntanglement()
            {
                int entanglement = 1;
                foreach (int present in Presents)
                {
                    entanglement *= present;
                }
                return entanglement;
            }

            public PresentGroup Clone()
            {
                PresentGroup group = new PresentGroup();
                group.Presents = new List<int>(Presents);
                return group;
            }
        }

        class PresentState : ISearchNode
        {
            PresentGroup[] PresentGroups = new PresentGroup[3];
            public int Cost { get; set; }
            public List<object> Actions { get; set; }
            public string VerboseInfo => $"Remaining presents: {remainingPresents.Count}    \n";

            public Queue<int> remainingPresents = new Queue<int>();
            public static int targetWeight;

            public PresentState()
            {
                for (int i = 0; i < 3; i++)
                {
                    PresentGroups[i] = new PresentGroup();
                }
                Actions = new List<object>();
            }


            private PresentState Clone()
            {
                PresentState state = new PresentState();

                for (int i = 0; i < 3; i++)
                {
                    state.PresentGroups[i] = PresentGroups[i].Clone();
                }

                state.remainingPresents = new Queue<int>(remainingPresents);

                state.Actions = new List<object>(Actions);
                state.Cost = Cost;
                return state;
            }

            private bool WeightCheck()
            {
                return PresentGroups.All(group => group.Weight == targetWeight);
            }

            private bool LegRoomCheck()
            {
                return PresentGroups[0].Count < PresentGroups[1].Count
                       && PresentGroups[0].Count < PresentGroups[2].Count;
            }

            public HashSet<ExpandAction> ExpandNode()
            {
                HashSet<ExpandAction> actions = new HashSet<ExpandAction>();
                
                for (int i = 0; i < 3; i++)
                {
                    if (remainingPresents.Count == 0)
                    {
                        return actions;
                    }
                    PresentState newState = this.Clone();
                    newState.PresentGroups[i].AddPresent(newState.remainingPresents.Dequeue());
                    if (newState.PresentGroups[i].Weight > targetWeight)
                    {
                        continue;
                    }

                    actions.Add(new ExpandAction
                                {
                                    action = i,
                                    cost = newState.PresentGroups[0].QuantumEntanglement,
                                    result = newState
                                });
                }

                return actions;
            }

            public bool Equals(ISearchNode otherState)
            {
                var otherPresentState = (PresentState) otherState;
                if (remainingPresents.Count != otherPresentState.remainingPresents.Count)
                {
                    return false;
                }
                return PresentGroups[0].Equals(otherPresentState.PresentGroups[0])
                       && PresentGroups[1].Equals(otherPresentState.PresentGroups[1])
                       && PresentGroups[2].Equals(otherPresentState.PresentGroups[2]);
            }

            public bool IsGoalState(ISearchNode goalState = null)
            {
                return remainingPresents.Count == 0 && WeightCheck() && LegRoomCheck();
            }

            public float GetHeuristic(ISearchNode goalState = null)
            {
                if (IsGoalState())
                {
                    return 0;
                }
                int heuristic = remainingPresents.Count * 10000;
                float difference = Math.Abs(PresentGroups[0].Weight - PresentGroups[1].Weight);
                difference += Math.Abs(PresentGroups[0].Weight - PresentGroups[2].Weight);
                difference += Math.Abs(PresentGroups[1].Weight - PresentGroups[2].Weight);
                difference *= 2;
                difference /= remainingPresents.Count;
                return heuristic + difference + (PresentGroups[0].QuantumEntanglement*1000);
            }
        }

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

        private HashSet<List<int>> MakeTriplets(List<int> presents)
        {
            HashSet<List<int>> combinations = new HashSet<List<int>>(new IntListEqualityComparer());
            Moep(combinations, new List<int>(), presents, 1);
            //int targetWeight = totalWeight / 3;
            
            return combinations;
        }


        private void Moep(HashSet<List<int>> combinations,  List<int> combo, List<int> presents,  int level)
        {
            if (presents.Count == 0)
            {
                return;
            }
            if (combo.Sum() > totalWeight/3)
            {
                return;
            }
            else if (combo.Sum() == totalWeight / 3)
            {
                combo.Sort();
                combinations.Add(combo);
                return;
            }
            else
            {
                if ((totalWeight/3) - combo.Sum() > presents.Sum())
                {
                    //not enough presents left to reach target weight
                    return;
                }
                foreach (int present in presents)
                {
                    List<int> newCombo = new List<int>(combo);
                    newCombo.Add(present);
                    int index = presents.IndexOf(present);
                    Moep(combinations, newCombo, presents.GetRange(index+1, presents.Count -(index+1)), level + 1);
                    
                    //int missing = totalWeight / 3 - combo.Sum();
                    //if (presents.Contains(missing))
                    //{

                    //}
                }
            }
        }

        private HashSet<Tuple<HashSet<int>, HashSet<int>, HashSet<int>>> GetCombinations(HashSet<List<int>> possiblePresentBags)
        {
            HashSet<Tuple<HashSet<int>, HashSet<int>, HashSet<int>>> ret = new HashSet<Tuple<HashSet<int>, HashSet<int>, HashSet<int>>>();
            List<HashSet<int>> possibleBags = possiblePresentBags.OrderBy(p => p.Count).Select(p => new HashSet<int>(p)).ToList();
            for (int i = 0; i < possibleBags.Count; i++)
            {
                HashSet<int> bag = possibleBags[i];
                List<HashSet<int>> remainingList = possibleBags.GetRange(i + 1, possibleBags.Count - (i + 1))
                    .Where(set => !set.Overlaps(bag)).ToList();

                for (int j = 0; j < remainingList.Count; j++)
                {
                    HashSet<int> bag2 = remainingList[j];
                    List<HashSet<int>> remainingList2 = remainingList.GetRange(j + 1, remainingList.Count - (j + 1))
                        .Where(set => !set.Overlaps(bag2)).ToList();

                    for (int k = 0; k < remainingList2.Count; k++)
                    {
                        HashSet<int> bag3 = remainingList2[k];

                        Tuple<HashSet<int>, HashSet<int>, HashSet<int>> combo = Tuple.Create(bag, bag2, bag3);
                        ret.Add(combo);
                    }
                }
            }

            return ret;
        }


        public override string GetSolutionPart1()
        {
            allPresents.AddRange(inputLines.Select(line => line.ToInt()));
            allPresents.Reverse();
            totalWeight = allPresents.Sum();
            HashSet<List<int>> possiblePresentBags = MakeTriplets(allPresents);
            HashSet<Tuple<HashSet<int>, HashSet<int>, HashSet<int>>> combinations = GetCombinations(possiblePresentBags);

            
            PresentState.targetWeight = totalWeight / 3;
            PresentState p = new PresentState {remainingPresents = new Queue<int>(allPresents)};
            int result = new AStar().GetMinimumCost(p, verbose: true);

            return result.ToString();
        }

        public override string GetSolutionPart2()
        {
            return base.GetSolutionPart2();
        }
    }
}