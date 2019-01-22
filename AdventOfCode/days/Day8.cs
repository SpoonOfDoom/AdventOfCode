using System.Text.RegularExpressions;

namespace AdventOfCode.Days
{
    // ReSharper disable once UnusedMember.Global
    public class Day8 : Day
    {
        public Day8() : base(8) { }

        private static int GetMemoryCharacters(string text)
        {
            text = text.Replace("\\\\", "1");
            text = Regex.Replace(text, @"\\x..", "1");
            text = text.Replace("\\\"", "1");
            return text.Length - 2;
        }

        private static int GetEncodedCharacters(string text)
        {
            text = text.Replace("\\", "\\\\");
            text = text.Replace("\"", "\\\"");
            return text.Length + 2;
        }
        protected override object GetSolutionPart1()
        {
            int sumChars = 0;
            int sumMemoryChars = 0;

            foreach (string line in InputLines)
            {
                sumChars += line.Length;
                sumMemoryChars += GetMemoryCharacters(line);
            }

            return (sumChars - sumMemoryChars).ToString();
        }

        protected override object GetSolutionPart2()
        {
            int sumChars = 0;
            int sumEncodedChars = 0;

            foreach (string line in InputLines)
            {
                sumChars += line.Length;
                sumEncodedChars += GetEncodedCharacters(line);
            }

            return (sumEncodedChars - sumChars).ToString();
        }
    }
}
