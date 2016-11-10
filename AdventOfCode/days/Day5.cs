using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Days
{
    public class Day5 : Day
    {
        public Day5() : base(5) { }
        
        private static bool IsNice(string myString)
        {
            if (myString.Contains("ab") || myString.Contains("cd") || myString.Contains("pq") || myString.Contains("xy"))
            {
                return false;
            }
            char? lastChar = null;
            bool doubleLetter = false;
            int vowelCount = 0;

            for (int i = 0; i < myString.Length; i++)
            {
                var c = myString[i];
                if (lastChar.HasValue && lastChar == c)
                {
                    doubleLetter = true;
                }


                if (c.ToString().IndexOfAny(new[] { 'a', 'e', 'i', 'o', 'u'}) >= 0)
                {
                    vowelCount++;
                }

                lastChar = c;
            }

            return doubleLetter && vowelCount >= 3;
        }

        private static bool IsNice2(string myString)
        {
            Regex regexBetween = new Regex(@"(\w)\w\1");
            Regex regexPair = new Regex(@"(\w\w)\w*\1");
            
            return regexBetween.IsMatch(myString) && regexPair.IsMatch(myString);
        }

        public override string GetSolutionPart1()
        {
            int niceCount = inputLines.Count(x => IsNice(x));
            return niceCount.ToString();
        }

        public override string GetSolutionPart2()
        {
            int niceCount = inputLines.Count(x => IsNice2(x));
            return niceCount.ToString();
        }
    }
}