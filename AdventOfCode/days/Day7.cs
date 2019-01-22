using System.Collections.Generic;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day7 : Day
    {
        public Day7() : base(7) { }

        private Dictionary<string, string> instructions = new Dictionary<string, string>();
        private Dictionary<string, int> instructionSolved = new Dictionary<string, int>();

        private Regex regNot = new Regex(@"^NOT (\w+)");
        private Regex regShift = new Regex(@"(\w+) (R|L)SHIFT (\d+)");
        private Regex regAndOr = new Regex(@"(\w+) (AND|OR) (\w+)");

        private int GetValue(string wire)
        {
            if (wire.IsNumeric())
            {
                return wire.ToInt();
            }

            if (instructionSolved.ContainsKey(wire)) //If we've already solved this wire, we don't need to do the recursive dance again, just use the result from last time
            {
                return instructionSolved[wire];
            }
            string wireInput = instructions[wire];

            if (wireInput.IsNumeric())
            {
                instructionSolved[wire] = wireInput.ToInt();
                return wireInput.ToInt();
            }
            else
            {
                if (regNot.IsMatch(wireInput))
                {
                    string w = regNot.Match(wireInput).Groups[1].Value;
                    int ret = ~GetValue(w);
                    instructionSolved[wire] = ret;
                    return ret;
                }
                else if (regShift.IsMatch(wireInput))
                {
                    var groups = regShift.Match(wireInput).Groups;
                    string val1 = groups[1].Value;
                    string val2 = groups[3].Value;

                    if (groups[2].Value == "R")
                    {
                        int ret = GetValue(val1) >> GetValue(val2);
                        instructionSolved[wire] = ret;
                        return ret;
                    }
                    else
                    {
                        int ret = GetValue(val1) << GetValue(val2);
                        instructionSolved[wire] = ret;
                        return ret;
                    }
                }
                else if (regAndOr.IsMatch(wireInput))
                {
                    var groups = regAndOr.Match(wireInput).Groups;
                    string val1 = groups[1].Value;
                    string val2 = groups[3].Value;
                    if (groups[2].Value == "AND")
                    {
                        int ret = GetValue(val1) & GetValue(val2);
                        instructionSolved[wire] = ret;
                        return ret;
                    }
                    else
                    {
                        int ret = GetValue(val1) | GetValue(val2);
                        instructionSolved[wire] = ret;
                        return ret;
                    }
                }
                else
                {
                    int ret = GetValue(wireInput);
                    instructionSolved[wire] = ret;
                    return ret;
                }
            }
        }

        protected override object GetSolutionPart1()
        {
            foreach (string line in InputLines) //For easy access, first parse the instructions into input and output parts
            {
                Regex regLine = new Regex(@"((\w|\d| )+) -> (\w+)");
                var groups = regLine.Match(line).Groups;

                string inputValue = groups[1].Value;
                string output = groups[3].Value;

                instructions[output] = inputValue;
            }
            var solution = GetValue("a");
            return solution.ToString();
        }

        protected override object GetSolutionPart2()
        {
            instructions["b"] = instructionSolved["a"].ToString();
            instructionSolved.Clear();
            var solution = GetValue("a");
            return solution.ToString();
        }
    }
}
