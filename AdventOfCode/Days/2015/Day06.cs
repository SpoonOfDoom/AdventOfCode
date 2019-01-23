using System;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions;

namespace AdventOfCode.Days._2015
{
    // ReSharper disable once UnusedMember.Global
    public class Day06 : Day
    {
        public Day06() : base(2015, 6) { }

        private bool[,] lightsBool = new bool[1000, 1000];
        private int[,] lightsInt = new int[1000, 1000];

        private struct Instruction
        {
            public string Action;
            public int X1, X2, Y1, Y2;

            public Instruction(string instruction)
            {
                Regex reg = new Regex(@"(toggle|turn o(n|ff)) (\d+),(\d+) through (\d+),(\d+)");
                var groups = reg.Match(instruction).Groups;

                Action = groups[1].Value;
                X1 = groups[3].Value.ToInt();
                Y1 = groups[4].Value.ToInt();
                X2 = groups[5].Value.ToInt();
                Y2 = groups[6].Value.ToInt();
            }
        }
        private void ExecuteInstruction(string text)
        {
            Instruction i = new Instruction(text);

            for (int x = i.X1; x <= i.X2; x++)
            {
                for (int y = i.Y1; y <= i.Y2; y++)
                {
                    switch (i.Action)
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

        private void ExecuteInstruction2(string text)
        {
            Instruction i = new Instruction(text);

            for (int x = i.X1; x <= i.X2; x++)
            {
                for (int y = i.Y1; y <= i.Y2; y++)
                {
                    switch (i.Action)
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

        protected override object GetSolutionPart1()
        {
            foreach (string line in InputLines)
            {
                ExecuteInstruction(line);
            }

            var test = from bool item in lightsBool
                where item
                select item;
            var lightCount = test.Count();

            return lightCount.ToString();
        }

        protected override object GetSolutionPart2()
        {
            foreach (string line in InputLines)
            {
                ExecuteInstruction2(line);
            }

            var test = from int item in lightsInt
                select item;
            var lightCount = test.Sum();

            return lightCount.ToString();
        }
    }
}
