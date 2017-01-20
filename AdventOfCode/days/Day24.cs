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
                return PresentGroups[0].Weight == PresentGroups[1].Weight
                       && PresentGroups[0].Weight == PresentGroups[2].Weight
                       && PresentGroups[1].Weight == PresentGroups[2].Weight;
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
                return WeightCheck() && LegRoomCheck();
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
        
        public override string GetSolutionPart1()
        {
            allPresents.AddRange(inputLines.Select(line => line.ToInt()));
            totalWeight = allPresents.Sum();
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