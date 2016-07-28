using System;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode.days
{
    /// <summary>
    /// Shared functions etc.
    /// </summary>
    public static class Util
    {

    }
    
    /// <summary>
    /// Exception for the case of an empty input string.
    /// </summary>
    public class InputEmptyException : Exception
    {
        const string defaultMessage = "The input string is null or empty.";
        public  InputEmptyException() : base(defaultMessage) { }
        public  InputEmptyException(string message) : base(message) { }
        public  InputEmptyException(string message, Exception inner) : base(message, inner) { }
        protected  InputEmptyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { }
    }

    public abstract class day
    {
        protected string input;
        public int Number;
        
        public day(int number)
        {
            this.Number = number;
            this.input = this.getInput();
        }

        public day(int number, string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new InputEmptyException();
            }
            this.input = input;
        }

        /// <summary>
        /// Input will be entered in a seperate method so that it can be collapsed individually (for bigger inputs)
        /// </summary>
        /// <returns></returns>

        public string getInput()
        {
            return File.ReadAllText("input\\day" + this.Number + ".txt");
        }

        public virtual string getSolutionPart1()
        {
            throw new NotImplementedException();
        }
        public virtual string getSolutionPart2()
        {
            throw new NotImplementedException();
        }
    }

    public class Day4 : day
    {
        const int number = 4;
        public Day4() : base(number) { }
        public Day4(string input) : base(number, input) { }
        
        static System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
        static StringBuilder sb = new StringBuilder();
        

        private static string getHash(string input)
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

        private int getNumberToMatch(string hashStart)
        {
            string hash = "";
            int i = 0;
            while (!hash.StartsWith(hashStart))
            {
                hash = getHash(input + i);
                if (hash.StartsWith(hashStart))
                {
                    break;
                }
                i++;
            }

            return i;
        }
        public override string getSolutionPart1()
        {
            return getNumberToMatch("00000").ToString();
        }

        public override string getSolutionPart2()
        {
            return getNumberToMatch("000000").ToString();
        }

        
    }

    public class Day5 : day
    {
        const int number = 5;
        public Day5() : base(number) { }
        public Day5(string input) : base(number, input) { }
        
        private bool isNice(string myString)
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


                if (c.ToString().IndexOfAny(new char[] { 'a', 'e', 'i', 'o', 'u'}) >= 0)
                {
                    vowelCount++;
                }

                lastChar = c;
            }

            return doubleLetter && vowelCount >= 3;
        }

        public override string getSolutionPart1()
        {
            var lines = input.Replace("\r", "").Split('\n').ToList();
            int niceCount = lines.Count(x => isNice(x));
            return niceCount.ToString();
        }

        public override string getSolutionPart2()
        {
            throw new NotImplementedException();
        }
    }
}
