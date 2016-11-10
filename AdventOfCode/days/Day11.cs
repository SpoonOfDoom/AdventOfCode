using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Days
{
    public class Day11 : Day
    {
        public Day11() : base(11) { }

        private string result1;
        
        private static string IteratePassword(string password)
        {
            StringBuilder sb = new StringBuilder(password);

            for (int i = sb.Length - 1; i >= 0; i--)
            {
                if (sb[i] == 'z')
                {
                    sb[i] = 'a';
                    if (i == 0)
                    {
                        sb.Insert(0, 'a');
                    }
                }
                else
                {
                    sb[i]++;
                    break;
                }
            }

            return sb.ToString();
        }

        private static bool IsValidPassword(string password)
        {
            Regex regPair = new Regex(@"(\w)\1");

            var matches = regPair.Matches(password);
            if (matches.Count <= 1)
            {
                return false;
            }

            if (password.IndexOfAny(new[] { 'i', 'o', 'l' }) >= 0)
            {
                return false;
            }

            int straightCount = 0;
            for (int i = 1; i < password.Length; i++)
            {
                if (password[i] == password[i - 1] + 1)
                {
                    straightCount++;
                    if (straightCount >= 2)
                    {
                        return true;
                    }
                }
                else
                {
                    straightCount = 0;
                }
            }
            
            return false;
        }

        public override string GetSolutionPart1()
        {
            string password = input;

            while (!IsValidPassword(password))
            {
                password = IteratePassword(password);
            }
            result1 = password;
            return password;
        }

        public override string GetSolutionPart2()
        {
            string password = IteratePassword(result1);

            while (!IsValidPassword(password))
            {
                password = IteratePassword(password);
            }
            return password;
        }
    }
}