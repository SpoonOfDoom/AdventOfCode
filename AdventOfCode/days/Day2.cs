using System;
using System.Linq;
using AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day2 : Day
    {
        public Day2() : base(2) { }

        private int getSurface(string dimensions)
        {
            var dim = dimensions.Split('x').Select(d => StringExtensions.ToInt(d)).ToList();
            return (2 * dim[0] * dim[1])
                   + (2 * dim[0] * dim[2])
                   + (2 * dim[1] * dim[2])
                   + Math.Min( dim[0] * dim[1], 
                       Math.Min(dim[1] * dim[2],
                           dim[2] * dim[0]
                           )
                       );
        }

        private int getRibbon(string dimensions)
        {
            var dim = dimensions.Split('x').Select(d => d.ToInt()).ToList();
            dim.Sort();
            var smallest = dim.Take(2).ToList();

            return 2 * smallest[0] + 2 * smallest[1] + dim[0] * dim[1] * dim[2];
        }

        public override string GetSolutionPart1()
        {
            int sum = 0;
            foreach (string line in inputLines)
            {
                sum += getSurface(line);
            }
            return sum.ToString();
        }

        public override string GetSolutionPart2()
        {
            int sum = 0;
            foreach (string line in inputLines)
            {
                sum += getRibbon(line);
            }
            return sum.ToString();
        }
    }
}