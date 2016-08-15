using System;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day6 : Day
    {
        public Day6() : base(6) { }

        private bool[,] lightsBool = new bool[1000, 1000];
        private int[,] lightsInt = new int[1000, 1000];

        private struct Instruction
        {
            public string action;
            public int x1, x2, y1, y2;

            public Instruction(string instruction)
            {
                Regex reg = new Regex(@"(toggle|turn o(n|ff)) (\d+),(\d+) through (\d+),(\d+)");
                var groups = reg.Match(instruction).Groups;

                action = groups[1].Value;
                x1 = groups[3].Value.ToInt();
                y1 = groups[4].Value.ToInt();
                x2 = groups[5].Value.ToInt();
                y2 = groups[6].Value.ToInt();
            }
        }
        private void executeInstruction(string text)
        {
            Instruction i = new Instruction(text);

            for (int x = i.x1; x <= i.x2; x++)
            {
                for (int y = i.y1; y <= i.y2; y++)
                {
                    switch (i.action)
                    {
                        case "turn on":
                            lightsBool[x, y] = true;
                            break;
                        case "turn off":
                            lightsBool[x, y] = false;
                            break;
                        case "toggle":
                            lightsBool[x, y] = !lightsBool[x, y];
                            break;

                        default:
                            throw new Exception("Instruction action not parsed properly");
                    }
                }
            }
        }

        private void executeInstruction2(string text)
        {
            Instruction i = new Instruction(text);

            for (int x = i.x1; x <= i.x2; x++)
            {
                for (int y = i.y1; y <= i.y2; y++)
                {
                    switch (i.action)
                    {
                        case "turn on":
                            lightsInt[x, y]++;
                            break;
                        case "turn off":
                            lightsInt[x, y] = Math.Max(lightsInt[x,y]-1, 0);

                            break;
                        case "toggle":
                            lightsInt[x, y] = lightsInt[x, y] + 2;
                            break;

                        default:
                            throw new Exception("Instruction action not parsed properly");
                    }
                }
            }
        }

        public override string GetSolutionPart1()
        {
            foreach (string line in inputLines)
            {
                executeInstruction(line);
            }

            var test = from bool item in lightsBool
                where item
                select item;
            var lightCount = test.Count();

            return lightCount.ToString();
        }

        public override string GetSolutionPart2()
        {
            foreach (string line in inputLines)
            {
                executeInstruction2(line);
            }

            var test = from int item in lightsInt
                select item;
            var lightCount = test.Sum();

            return lightCount.ToString();
        }
    }
}