using System.Text;

namespace AdventOfCode.Days
{
    public class Day10 : Day
    {
        public Day10() : base(10) { }

        private string getNewSequence(string sequence)
        {
            StringBuilder sb = new StringBuilder();
            char? n = null;
            int count = 0;
            for (int i = 0; i < sequence.Length; i++)
            {
                if (i == 0 || n == sequence[i])
                {
                    if (i == 0)
                    {
                        n = sequence[i];
                    }
                    count++;
                }
                else
                {
                    sb.Append(count);
                    sb.Append(n);
                    count = 1;
                    n = sequence[i];
                }
                if (i == sequence.Length - 1)
                {
                    sb.Append(count);
                    sb.Append(n);
                }
            }
            return sb.ToString();
        }
        
        public override string GetSolutionPart1()
        {
            string result = input;
            for (int i = 0; i < 40; i++)
            {
                result = getNewSequence(result);
            }
            return result.Length.ToString();
        }

        public override string GetSolutionPart2()
        {
            string result = input;
            for (int i = 0; i < 50; i++)
            {
                result = getNewSequence(result);
            }
            return result.Length.ToString();
        }
    }
}