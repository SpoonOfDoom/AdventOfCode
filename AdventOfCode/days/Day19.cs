using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Priority_Queue;
// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace AdventOfCode.Days
{
    public class Day19 : Day
    {
        private class MyNode
        {
            public string Text { get; set; }
            public int Cost { get; set; }
            public int Heuristic { get; set; }
            public int TentativeCost => Cost + Heuristic;
        }

        public Day19() : base(19) { }

        private Regex replacementRegex = new Regex(@"^(\w+) => (\w+)$");
        
        private Dictionary<string, HashSet<string>> replacements = new Dictionary<string, HashSet<string>>();
        private string medicineMolecule;
        private HashSet<string> possibleMolecules = new HashSet<string>();

        private SimplePriorityQueue<MyNode, int> openList = new SimplePriorityQueue<MyNode, int>();
        private HashSet<MyNode> closedList = new HashSet<MyNode>();

        private List<string> medicineAtoms = new List<string>();

        private Stopwatch searchWatch = new Stopwatch();

        private void ParseLines()
        {
            foreach (string line in inputLines)
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
        }

        private HashSet<string> GetReplacementsFor(string atom, string molecule = null, StringBuilder builder = null)
        {
            if (builder == null)
            {
                builder = new StringBuilder();
            }
            if (molecule == null)
            {
                molecule = medicineMolecule;
            }
            var retSet = new HashSet<string>();
            var rep = replacements[atom];
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
                        builder.Clear();
                        
                        builder.Append(beginning);
                        builder.Append(replacement);
                        builder.Append(end);

                        retSet.Add(builder.ToString());
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

        private int CompareMolecules(List<string> atoms)
        {
            int diffCount = 0;
            int length;
            int lengthDifference = 0;
            //var atoms = GetAtoms(a);

            if (atoms.Count == medicineAtoms.Count)
            {
                length = atoms.Count;
            }
            else
            {
                length = Math.Min(atoms.Count, medicineAtoms.Count);
                lengthDifference = Math.Abs(atoms.Count - medicineAtoms.Count);
            }
            int chainBonus = 0;
            for (int i = 0; i < length; i++)
            {
                if (atoms[i] != medicineAtoms[i])
                {
                    diffCount += 2;
                    diffCount -= chainBonus;
                    chainBonus = 0;
                    if (!replacements.ContainsKey(atoms[i]))
                    {
                        diffCount+=10; //atom is wrong here and cannot be replaced at all, so priority takes a big hit
                    }
                    else
                    {
                        if (replacements[atoms[i]].Any(r => r.StartsWith(medicineAtoms[i])))
                        {
                            diffCount--; //atom is wrong here, but can be replaced into the right one, so priority is a bit better
                        }
                    }
                }
                else
                {
                    chainBonus+=2;
                }
            }

            int priority = diffCount;
            if (medicineAtoms.Count - atoms.Count == 1)
            {
                priority -= 10;
            }
            else if (atoms.Count > medicineAtoms.Count)
            {
                priority += 100000000; //Can't replace atoms to become less
            }
            else
            {
                priority += (lengthDifference*10);
            }
            return priority;
        }

        private int GetHeuristicFor(string molecule)
        {
            var atoms = GetAtoms(molecule);
            if (atoms.Count > medicineAtoms.Count)
            {
                return 999999999;
            }
            return molecule == medicineMolecule ? 0 : CompareMolecules(atoms);
        }

        private HashSet<MyNode> ExpandNode(MyNode node)
        {
            HashSet<MyNode> newNodes = new HashSet<MyNode>();
            HashSet<string> newMolecules = new HashSet<string>();
            foreach (string atom in replacements.Keys)
            {
                newMolecules.UnionWith(GetReplacementsFor(atom, node.Text));
            }

            foreach (string newMolecule in newMolecules)
            {
                if (GetAtoms(newMolecule).Count > medicineAtoms.Count)
                {
                    continue; //Don't even add them to the openList, they're impossible anyway
                }
                var newNode = new MyNode()
                {
                    Cost = node.Cost + 1,
                    Heuristic = GetHeuristicFor(newMolecule),
                    Text = newMolecule
                };
                newNodes.Add(newNode);
            }
            return newNodes;
        }

        private int GetRoute(string start, string target)
        {
            searchWatch.Start();
            MyNode node = new MyNode()
            {
                Cost = 0,
                Heuristic = GetHeuristicFor(start),
                Text = start
            };
            
            openList.Enqueue(node, 0);

            TimeSpan? timeTo1000 = null;
            TimeSpan? timeTo2000 = null;
            bool useParallel = false;
            while (openList.Count > 0)
            {
                MyNode current = openList.Dequeue();

                if (current.Text == target)
                {
                    return current.Cost;
                }

                closedList.Add(current);
                if (closedList.Count == 1000 && !timeTo1000.HasValue)
                {
                    timeTo1000 = searchWatch.Elapsed;
                }
                if (closedList.Count == 2000 && !timeTo2000.HasValue)
                {
                    timeTo2000 = searchWatch.Elapsed;
                }
                var newNodes = ExpandNode(current);
                
                Console.Clear();
                Console.WriteLine("Open list: {0}", openList.Count);
                Console.WriteLine("Closed list: {0}", closedList.Count);
                if (openList.Count > 0) Console.WriteLine("First tentative cost: {0}", openList.First.TentativeCost);
                if (timeTo1000.HasValue)
                {
                    Console.WriteLine("Time to 1000 closed: {0}", timeTo1000);
                }
                if (timeTo2000.HasValue)
                {
                    Console.WriteLine("Time to 2000 closed: {0}", timeTo2000);
                }
                Console.WriteLine("Time: {0}:{1}:{2}.{3}", searchWatch.Elapsed.Hours, searchWatch.Elapsed.Minutes, searchWatch.Elapsed.Seconds, searchWatch.Elapsed.Milliseconds);
                
                Parallel.ForEach(newNodes, (n) =>
                {
                    lock (openList)
                    {
                        if (openList.Any(x => x.Text == n.Text && x.TentativeCost > n.TentativeCost))
                        {
                            openList.UpdatePriority(n, n.TentativeCost);
                        }
                        else
                        {
                            if (closedList.All(x => x.Text != n.Text))
                            {
                                if (n.Heuristic == 999999999)
                                {
                                    closedList.Add(n);
                                }
                                else
                                {
                                    openList.Enqueue(n, n.TentativeCost);
                                }
                            }
                        }
                    }
                });
                //foreach (var n in newNodes)
                //{
                //    if (openList.Any(x => x.Text == n.Text && x.TentativeCost > n.TentativeCost))
                //    {
                //        openList.UpdatePriority(n, n.TentativeCost);
                //        continue;
                //    }
                //    if (closedList.All(x => x.Text != n.Text))
                //    {
                //        if (n.Heuristic == 999999999)
                //        {
                //            closedList.Add(n);
                //        }
                //        else
                //        {
                //            openList.Enqueue(n, n.TentativeCost);
                //        }
                //    }
                //    else
                //    {
                //        Debug.WriteLine("Node {0} already in closedList", n.Text);
                //    }
                //}
            }
            return -1;
        }

        public override string GetSolutionPart1()
        {
            ParseLines();
            
            foreach (string atom in replacements.Keys)
            {
                possibleMolecules.UnionWith(GetReplacementsFor(atom));
            }
            return possibleMolecules.Count.ToString();
        }

        public override string GetSolutionPart2()
        {
            medicineAtoms = GetAtoms(medicineMolecule);
            int result = GetRoute("e", medicineMolecule);
            return result.ToString();
        }
    }
}