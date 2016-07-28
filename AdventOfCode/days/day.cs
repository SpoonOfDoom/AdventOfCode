using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

    public abstract class Day
    {
        protected string input;
        protected List<string> inputLines;
        public int Number;
        
        public Day(int number)
        {
            this.Number = number;
            this.getInput();
        }

        public Day(int number, string input)
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

        public void getInput()
        {
            this.input = File.ReadAllText("input\\day" + this.Number + ".txt");
            this.inputLines = File.ReadAllLines("input\\day" + this.Number + ".txt").ToList();
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

    public class Day4 : Day
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

    public class Day5 : Day
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

        private bool isNice2(string myString)
        {
            Regex regexBetween = new Regex(@"(\w)\w\1");
            Regex regexPair = new Regex(@"(\w\w)\w*\1");
            
            return regexBetween.IsMatch(myString) && regexPair.IsMatch(myString);
        }

        public override string getSolutionPart1()
        {
            int niceCount = inputLines.Count(x => isNice(x));
            return niceCount.ToString();
        }

        public override string getSolutionPart2()
        {
            int niceCount = inputLines.Count(x => isNice2(x));
            return niceCount.ToString();
        }
    }

    public class Day6 : Day
    {
        const int number = 6;
        public Day6() : base(number) { }

        private bool[,] lightsBool = new bool[1000, 1000];
        private int[,] lightsInt = new int[1000, 1000];

        private struct Instruction
        {
            public string action;
            public int x1, x2, y1, y2;

            public Instruction(string instruction)
            {
                Regex reg = new Regex(@"(toggle|turn o(n|ff)) (\d+),(\d+) through (\d+),(\d+)");
                var groups = reg.Match(instruction).Groups;

                action = groups[1].Value;
                x1 = int.Parse(groups[3].Value);
                y1 = int.Parse(groups[4].Value);
                x2 = int.Parse(groups[5].Value);
                y2 = int.Parse(groups[6].Value);
            }
        }
        private void executeInstruction(string text)
        {
            Instruction i = new Instruction(text);

            for (int x = i.x1; x <= i.x2; x++)
            {
                for (int y = i.y1; y <= i.y2; y++)
                {
                    switch (i.action)
                    {
                        case "turn on":
                            lightsBool[x, y] = true;
                            break;
                        case "turn off":
                            lightsBool[x, y] = false;
                            break;
                        case "toggle":
                            lightsBool[x, y] = !lightsBool[x, y];
                            break;

                        default:
                            throw new Exception("Instruction action not parsed properly");
                            break;
                    }
                }
            }
        }

        private void executeInstruction2(string text)
        {
            Instruction i = new Instruction(text);

            for (int x = i.x1; x <= i.x2; x++)
            {
                for (int y = i.y1; y <= i.y2; y++)
                {
                    switch (i.action)
                    {
                        case "turn on":
                            lightsInt[x, y]++;
                            break;
                        case "turn off":
                            lightsInt[x, y] = Math.Max(lightsInt[x,y]-1, 0);

                            break;
                        case "toggle":
                            lightsInt[x, y] = lightsInt[x, y] + 2;
                            break;

                        default:
                            throw new Exception("Instruction action not parsed properly");
                            break;
                    }
                }
            }
        }

        public override string getSolutionPart1()
        {
            foreach (string line in inputLines)
            {
                executeInstruction(line);
            }

            int lightCount = 0;
            var test = from bool item in lightsBool
                       where item
                       select item;
            lightCount = test.Count();

            return lightCount.ToString();
        }

        public override string getSolutionPart2()
        {
            foreach (string line in inputLines)
            {
                executeInstruction2(line);
            }

            int lightCount = 0;
            var test = from int item in lightsInt
                       select item;
            lightCount = test.Sum();

            return lightCount.ToString();
        }
    }

    class Day7 : Day
    {
        const int number = 7;
        public Day7() : base(number) { }

        public override string getSolutionPart1()
        {
            return base.getSolutionPart1();
        }

        public override string getSolutionPart2()
        {
            return base.getSolutionPart2();
        }
    }
}
