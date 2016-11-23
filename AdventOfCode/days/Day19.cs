using System;
using AdventOfCode.Extensions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Priority_Queue;

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
            
            StringBuilder b = new StringBuilder(molecule);
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

        private int CompareMolecules(string a)
        {
            int diffCount = 0;
            int length;
            int lengthDifference = 0;
            var atoms = GetAtoms(a);

            if (atoms.Count == medicineAtoms.Count)
            {
                length = atoms.Count;
            }
            else
            {
                length = Math.Min(atoms.Count, medicineAtoms.Count);
                lengthDifference = Math.Abs(atoms.Count - medicineAtoms.Count);
            }

            for (int i = 0; i < length; i++)
            {
                if (atoms[i] != medicineAtoms[i])
                {
                    diffCount++;
                }
            }
            return diffCount + (lengthDifference*2);
        }

        private int GetHeuristicFor(string molecule, string target)
        {
            return molecule == target ? 0 : CompareMolecules(molecule);
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
                var newNode = new MyNode()
                {
                    Cost = node.Cost + 1,
                    Heuristic = GetHeuristicFor(newMolecule, medicineMolecule),
                    Text = newMolecule
                };
                newNodes.Add(newNode);
            }
            return newNodes;
        }

        private int GetRoute(string start, string target)
        {
            MyNode node = new MyNode()
            {
                Cost = 0,
                Heuristic = GetHeuristicFor(start, target),
                Text = start
            };
            
            openList.Enqueue(node, 0);



            while (openList.Count > 0)
            {
                MyNode current = openList.Dequeue();

                if (current.Text == target)
                {
                    return current.Cost;
                }

                closedList.Add(current);
                var newNodes = ExpandNode(current);
                foreach (var n in newNodes)
                {
                    if (openList.Contains(n) && openList.Any(x => x.TentativeCost > n.TentativeCost))
                    {
                        openList.UpdatePriority(n, n.TentativeCost);
                        continue;
                    }
                    if (!closedList.Contains(n))
                    {
                        openList.Enqueue(n, n.TentativeCost);
                    }
                    else
                    {
                        Debug.WriteLine("Node {0} already in closedList", n.Text);
                    }
                }
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