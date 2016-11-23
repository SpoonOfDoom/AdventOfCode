using System;
using AdventOfCode.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace AdventOfCode.Days
{
    public class Day19 : Day
    {
        public Day19() : base(19) { }
        private Regex replacementRegex = new Regex(@"^(\w+) => (\w+)$");
        
        private Dictionary<string, HashSet<string>> replacements = new Dictionary<string, HashSet<string>>();
        private string medicineMolecule;
        private HashSet<string> possibleMolecules = new HashSet<string>();

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

        private HashSet<string> GetReplacementsFor(string atom, StringBuilder builder = null)
        {
            if (builder == null)
            {
                builder = new StringBuilder();
            }
            var retSet = new HashSet<string>();
            var rep = replacements[atom];
            Regex regex = new Regex(atom);
            var matches = regex.Matches(medicineMolecule);

            if (matches.Count > 1)
            {
                for (int i = 0; i < matches.Count; i++)
                {
                    int start = matches[i].Index;
                    int length = matches[i].Value.Length;

                    string beginning = medicineMolecule.Substring(0, start);
                    string end = medicineMolecule.Substring(start + length);

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
            throw new NotImplementedException();
        }
    }
}