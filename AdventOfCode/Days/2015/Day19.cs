using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using AdventOfCode.Tools;

// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace AdventOfCode.Days._2015
{
    // ReSharper disable once UnusedMember.Global
    public class Day19 : Day
    {
        public Day19() : base(2015, 19) {}

        private static Regex replacementRegex = new Regex(@"^(\w+) => (\w+)$");
        
        private static Dictionary<string, HashSet<string>> replacements = new Dictionary<string, HashSet<string>>();
        private static Dictionary<string, HashSet<string>> replacementsReverse = new Dictionary<string, HashSet<string>>();
        private static string medicineMolecule;
        private static HashSet<string> possibleMolecules = new HashSet<string>();

        private static List<string> medicineAtoms = new List<string>();


        public class MoleculeState : ISearchNode
        {
            public string Text { get; set; }
            public int Cost { get; set; }
            public List<object> Actions { get; set; }
            public string VerboseInfo { get; }
            public string StringHash => Text;
            public long NumericHash { get; }
            

            public HashSet<ExpandAction> ExpandNode()
            {
                HashSet<ExpandAction> actions = new HashSet<ExpandAction>();
                string molecule = Text;

                foreach (string atom in replacementsReverse.Keys)
                {
                    HashSet<string> rep = replacementsReverse[atom];
                    Regex regex = new Regex(atom);
                    MatchCollection matches = regex.Matches(molecule);
                    if (matches.Count > 0)
                    {
                        for (int i = 0; i < matches.Count; i++)
                        {
                            int start = matches[i].Index;
                            int length = matches[i].Value.Length;

                            string beginning = molecule.Substring(0, start);
                            string end = molecule.Substring(start + length);
                            
                            foreach (string replacement in rep)
                            {
                                var newNode = new MoleculeState
                                {
                                    Text = beginning + replacement + end,
                                    Actions = new List<object>(Actions)
                                };
                                var expandAction = new ExpandAction
                                {
                                    Cost = 1,
                                    Action = replacement,
                                    Result = newNode
                                };
                                actions.Add(expandAction);
                            }
                            
                        }
                    }
                }

                return actions;
            }

            public bool Equals(ISearchNode otherState)
            {
                return Text == ((MoleculeState)otherState).Text;
            }

            public bool IsGoalState(ISearchNode goalState = null)
            {
                return Equals(goalState);
            }

            public float GetHeuristic(ISearchNode goalState = null)
            {
                if (Text == "e")
                {
                    return 0;
                }
                List<string> atoms = GetAtoms(Text);
                return atoms.Count > medicineAtoms.Count ? 999999999 : CompareMolecules(atoms);
            }

            /// <summary>
            /// not needed in this case, Text property serves as Hash
            /// </summary>
            public void CreateHash() { }
        }

        private void ParseLines()
        {
            foreach (string line in InputLines)
            {
                if (line.Contains("=>"))
                {
                    var groups = replacementRegex.Match(line).Groups;

                    if (!replacements.ContainsKey(groups[1].Value))
                    {
                        replacements[groups[1].Value] = new HashSet<string>();
                    }
                    replacements[groups[1].Value].Add(groups[2].Value);
                }
                else
                {
                    medicineMolecule = line;
                }
            }

            foreach (var rule in replacements)
            {
                foreach (string molecule in rule.Value)
                {
                    if (!replacementsReverse.ContainsKey(molecule))
                    {
                        replacementsReverse[molecule] = new HashSet<string>();
                    }
                    replacementsReverse[molecule].Add(rule.Key);
                }
            }
        }

        private static HashSet<string> GetReplacementsFor(string atom, string molecule = null, bool reverse = false)
        {
            if (molecule == null)
            {
                molecule = medicineMolecule;
            }
            var retSet = new HashSet<string>();
            var rep = reverse ? replacementsReverse[atom] : replacements[atom];
            Regex regex = new Regex(atom);
            
            var matches = regex.Matches(molecule);

            if (matches.Count > 0)
            {
                for (int i = 0; i < matches.Count; i++)
                {
                    int start = matches[i].Index;
                    int length = matches[i].Value.Length;

                    string beginning = molecule.Substring(0, start);
                    string end = molecule.Substring(start + length);

                    foreach (string replacement in rep)
                    {
                        retSet.Add(beginning + replacement + end);
                    }
                }
            }
            return retSet;
        }

        private static List<string> GetAtoms(string molecule)
        {
            var retList = new List<string>();
            
            while (molecule.Length > 0)
            {
                string atom;
                if (molecule.Length >= 2 && char.IsLower(molecule[1]))
                {
                    atom = molecule.Substring(0, 2);
                }
                else
                {
                    atom = molecule.Substring(0, 1);
                }
                retList.Add(atom);
                molecule = molecule.Substring(atom.Length);
            }
            return retList;
        }

        private static int CompareMolecules(List<string> atoms)
        {
            int priority = 0;
            priority += (int) Math.Pow(atoms.Count, 3);
            int replacementPotential = 0;
            string atomString = string.Join("", atoms);
            foreach (string s in replacementsReverse.Keys)
            {
                if (atomString.Contains(s))
                {
                    replacementPotential += s.Length;
                }
            }
            priority -= replacementPotential;
            return priority;
        }

        protected override object GetSolutionPart1()
        {
            ParseLines();
            
            foreach (string atom in replacements.Keys)
            {
                possibleMolecules.UnionWith(GetReplacementsFor(atom));
            }
            return possibleMolecules.Count.ToString();
        }

        protected override object GetSolutionPart2()
        {
            medicineAtoms = GetAtoms(medicineMolecule);
            AStar aStar = new AStar();
            MoleculeState start = new MoleculeState {Text = medicineMolecule, Actions = new List<object>()};
            MoleculeState goal = new MoleculeState {Text = "e", Actions = new List<object>()};
            int result = aStar.GetMinimumCost(start, goal);
            return result.ToString();
        }
    }
}
