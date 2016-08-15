using System.Linq;

namespace AdventOfCode.Days
{
    public class Day1 : Day
    {
        public Day1() : base(1) { }

        public override string GetSolutionPart1()
        {
            int floor = 0;

            floor += input.Count(c => c == '(');
            floor -= input.Count(c => c == ')');

            return floor.ToString();
        }

        public override string GetSolutionPart2()
        {
            int floor = 0;
            int i;
            for (i = 0; i < input.Length; i++)
            {
                if (input[i] == '(')
                {
                    floor++;
                }
                else
                {
                    floor--;
                }
                if (floor < 0)
                {
                    break;
                }
            }
            return (i + 1).ToString();
        }
    }
}