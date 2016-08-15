using System.Collections.Generic;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day7 : Day
    {
        public Day7() : base(7) { }

        private Dictionary<string, int> wires = new Dictionary<string, int>();
        private Dictionary<string, string> instructions = new Dictionary<string, string>();
        private Dictionary<string, int> instructionSolved = new Dictionary<string, int>();

        private Regex regNot = new Regex(@"^NOT (\w+)");
        private Regex regShift = new Regex(@"(\w+) (R|L)SHIFT (\d+)");
        private Regex regAndOr = new Regex(@"(\w+) (AND|OR) (\w+)");

        private int getValue(string wire)
        {
            if (wire.IsNumeric())
            {
                return wire.ToInt();
            }

            if (instructionSolved.ContainsKey(wire)) //If we've already solved this wire, we don't need to do the recursive dance again, just use the result from last time
            {
                return instructionSolved[wire];
            }
            string input = instructions[wire];

            if (input.IsNumeric())
            {
                instructionSolved[wire] = input.ToInt();
                return input.ToInt();
            }
            else
            {
                if (regNot.IsMatch(input))
                {
                    string w = regNot.Match(input).Groups[1].Value;
                    int ret = ~getValue(w);
                    instructionSolved[wire] = ret;
                    return ret;
                }
                else if (regShift.IsMatch(input))
                {
                    var groups = regShift.Match(input).Groups;
                    string val1 = groups[1].Value;
                    string val2 = groups[3].Value;

                    if (groups[2].Value == "R")
                    {
                        int ret = getValue(val1) >> getValue(val2);
                        instructionSolved[wire] = ret;
                        return ret;
                    }
                    else
                    {
                        int ret = getValue(val1) << getValue(val2);
                        instructionSolved[wire] = ret;
                        return ret;
                    }
                }
                else if (regAndOr.IsMatch(input))
                {
                    var groups = regAndOr.Match(input).Groups;
                    string val1 = groups[1].Value;
                    string val2 = groups[3].Value;
                    if (groups[2].Value == "AND")
                    {
                        int ret = getValue(val1) & getValue(val2);
                        instructionSolved[wire] = ret;
                        return ret;
                    }
                    else
                    {
                        int ret = getValue(val1) | getValue(val2);
                        instructionSolved[wire] = ret;
                        return ret;
                    }
                }
                else
                {
                    int ret = getValue(input);
                    instructionSolved[wire] = ret;
                    return ret;
                }
            }
        }

        public override string GetSolutionPart1()
        {
            foreach (string line in inputLines) //For easy access, first parse the instructions into input and output parts
            {
                Regex regLine = new Regex(@"((\w|\d| )+) -> (\w+)");
                var groups = regLine.Match(line).Groups;

                string inputValue = groups[1].Value;
                string output = groups[3].Value;

                instructions[output] = inputValue;
            }
            var solution = getValue("a");
            return solution.ToString();
        }

        public override string GetSolutionPart2()
        {
            instructions["b"] = instructionSolved["a"].ToString();
            instructionSolved.Clear();
            var solution = getValue("a");
            return solution.ToString();
        }
    }
}