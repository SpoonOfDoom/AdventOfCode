using System.Text.RegularExpressions;

namespace AdventOfCode.Days
{
    public class Day8 : Day
    {
        public Day8() : base(8) { }

        private static int getMemoryCharacters(string text)
        {
            text = text.Replace("\\\\", "1");
            text = Regex.Replace(text, @"\\x..", "1");
            text = text.Replace("\\\"", "1");
            return text.Length - 2;
        }

        private static int getEncodedCharacters(string text)
        {
            text = text.Replace("\\", "\\\\");
            text = text.Replace("\"", "\\\"");
            return text.Length + 2;
        }
        public override string GetSolutionPart1()
        {
            int sumChars = 0;
            int sumMemoryChars = 0;

            foreach (string line in inputLines)
            {
                sumChars += line.Length;
                sumMemoryChars += getMemoryCharacters(line);
            }

            return (sumChars - sumMemoryChars).ToString();
        }

        public override string GetSolutionPart2()
        {
            int sumChars = 0;
            int sumEncodedChars = 0;

            foreach (string line in inputLines)
            {
                sumChars += line.Length;
                sumEncodedChars += getEncodedCharacters(line);
            }

            return (sumEncodedChars - sumChars).ToString();
        }
    }
}