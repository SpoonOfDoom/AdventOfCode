using System.Linq;

namespace AdventOfCode.Days
{
    // ReSharper disable once UnusedMember.Global
    public class Day01 : Day
    {
        public Day01() : base(1) { }

        protected override object GetSolutionPart1()
        {
            int floor = 0;

            floor += Input.Count(c => c == '(');
            floor -= Input.Count(c => c == ')');

            return floor.ToString();
        }

        protected override object GetSolutionPart2()
        {
            int floor = 0;
            int i;
            for (i = 0; i < Input.Length; i++)
            {
                if (Input[i] == '(')
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
