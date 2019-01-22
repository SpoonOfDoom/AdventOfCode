using System.Text;

namespace AdventOfCode.Days
{
    public class Day4 : Day
    {
        public Day4() : base(4) { }
        
        static System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
        static StringBuilder sb = new StringBuilder();
        

        private static string GetHash(string input)
        {
            // step 1, calculate MD5 hash from input
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            // step 2, convert byte array to hex string
            sb.Clear();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }

        private int GetNumberToMatch(string hashStart)
        {
            string hash = "";
            int i = 0;
            while (!hash.StartsWith(hashStart))
            {
                hash = GetHash(Input + i);
                if (hash.StartsWith(hashStart))
                {
                    break;
                }
                i++;
            }

            return i;
        }
        protected override object GetSolutionPart1()
        {
            return GetNumberToMatch("00000").ToString();
        }

        protected override object GetSolutionPart2()
        {
            return GetNumberToMatch("000000").ToString();
        }
    }
}
