﻿using AdventOfCode.Extensions;
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

    public class Day1 : Day
    {
        const int number = 1;

        public Day1() : base(number) { }

        public override string getSolutionPart1()
        {
            int floor = 0;

            floor += input.Count(c => c == '(');
            floor -= input.Count(c => c == ')');

            return floor.ToString();
        }

        public override string getSolutionPart2()
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

    public class Day2 : Day
    {
        const int number = 2;
        public Day2() : base(number) { }

        private int getSurface(string dimensions)
        {
            var dim = dimensions.Split('x').Select(d => d.ToInt()).ToList();
            return (2 * dim[0] * dim[1])
                    + (2 * dim[0] * dim[2])
                    + (2 * dim[1] * dim[2])
                    + Math.Min( dim[0] * dim[1], 
                                Math.Min(dim[1] * dim[2],
                                           dim[2] * dim[0]
                                        )
                               );
        }

        private int getRibbon(string dimensions)
        {
            var dim = dimensions.Split('x').Select(d => d.ToInt()).ToList();
            dim.Sort();
            var smallest = dim.Take(2).ToList();

            return 2 * smallest[0] + 2 * smallest[1] + dim[0] * dim[1] * dim[2];
        }

        public override string getSolutionPart1()
        {
            int sum = 0;
            foreach (string line in inputLines)
            {
                sum += getSurface(line);
            }
            return sum.ToString();
        }

        public override string getSolutionPart2()
        {
            int sum = 0;
            foreach (string line in inputLines)
            {
                sum += getRibbon(line);
            }
            return sum.ToString();
        }
    }

    public class Day3 : Day
    {
        const int number = 3;

        public Day3() : base(number) { }

        

        public override string getSolutionPart1()
        {
            HashSet<string> hashset = new HashSet<string>();
            int x = 0;
            int y = 0;
            hashset.Add(x + "/" + y);
            for (int i = 0; i < input.Length; i++)
            {
                switch (input[i])
                {
                    case '^':
                        y++;
                        break;
                    case 'v':
                        y--;
                        break;
                    case '<':
                        x--;
                        break;
                    case '>':
                        x++;
                        break;
                }
                hashset.Add(x + "/" + y);
            }

            return hashset.Count.ToString();
        }

        public override string getSolutionPart2()
        {
            HashSet<string> hashset = new HashSet<string>();
            int x1 = 0;
            int x2 = 0;
            int y1 = 0;
            int y2 = 0;
            hashset.Add(x1 + "/" + y1); //add starting location
            bool robo = false;
            for (int i = 0; i < input.Length; i++)
            {
                switch (input[i])
                {
                    case '^':
                        if (robo)
                        {
                            y2++;
                        }
                        else
                        {
                            y1++;
                        }
                        break;
                    case 'v':
                        if (robo)
                        {
                            y2--;
                        }
                        else
                        {
                            y1--;
                        }
                        break;
                    case '<':
                        if (robo)
                        {
                            x2--;
                        }
                        else
                        {
                            x1--;
                        }
                        break;
                    case '>':
                        if (robo)
                        {
                            x2++;
                        }
                        else
                        {
                            x1++;
                        }
                        break;
                }
                if (robo)
                {
                    hashset.Add(x2 + "/" + y2);
                }
                else
                {
                    hashset.Add(x1 + "/" + y1);
                }
                robo = !robo;
            }

            return hashset.Count.ToString();
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
                x1 = groups[3].Value.ToInt();
                y1 = groups[4].Value.ToInt();
                x2 = groups[5].Value.ToInt();
                y2 = groups[6].Value.ToInt();
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

    public class Day7 : Day
    {
        const int number = 7;
        public Day7() : base(number) { }

        Dictionary<string, int> wires = new Dictionary<string, int>();
        Dictionary<string, string> instructions = new Dictionary<string, string>();
        Dictionary<string, int> instructionSolved = new Dictionary<string, int>();

        Regex regNot = new Regex(@"^NOT (\w+)");
        Regex regShift = new Regex(@"(\w+) (R|L)SHIFT (\d+)");
        Regex regAndOr = new Regex(@"(\w+) (AND|OR) (\w+)");

        private int getValue(string wire)
        {
            if (wire.IsNumeric())
            {
                return wire.ToInt();
            }

            if (instructionSolved.ContainsKey(wire)) //If we've already solved this wire, we don't need to do the recursive dance again, just use the result from last time
            {
                return instructionSolved[wire];
            }
            string input = instructions[wire];

            if (input.IsNumeric())
            {
                instructionSolved[wire] = input.ToInt();
                return input.ToInt();
            }
            else
            {
                if (regNot.IsMatch(input))
                {
                    string w = regNot.Match(input).Groups[1].Value;
                    int ret = ~getValue(w);
                    instructionSolved[wire] = ret;
                    return ret;
                }
                else if (regShift.IsMatch(input))
                {
                    var groups = regShift.Match(input).Groups;
                    string val1 = groups[1].Value;
                    string val2 = groups[3].Value;

                    if (groups[2].Value == "R")
                    {
                        int ret = getValue(val1) >> getValue(val2);
                        instructionSolved[wire] = ret;
                        return ret;
                    }
                    else
                    {
                        int ret = getValue(val1) << getValue(val2);
                        instructionSolved[wire] = ret;
                        return ret;
                    }
                }
                else if (regAndOr.IsMatch(input))
                {
                    var groups = regAndOr.Match(input).Groups;
                    string val1 = groups[1].Value;
                    string val2 = groups[3].Value;
                    if (groups[2].Value == "AND")
                    {
                        int ret = getValue(val1) & getValue(val2);
                        instructionSolved[wire] = ret;
                        return ret;
                    }
                    else
                    {
                        int ret = getValue(val1) | getValue(val2);
                        instructionSolved[wire] = ret;
                        return ret;
                    }
                }
                else
                {
                    int ret = getValue(input);
                    instructionSolved[wire] = ret;
                    return ret;
                }
            }
        }

        public override string getSolutionPart1()
        {
            foreach (string line in inputLines) //For easy access, first parse the instructions into input and output parts
            {
                Regex regLine = new Regex(@"((\w|\d| )+) -> (\w+)");
                var groups = regLine.Match(line).Groups;

                string input = groups[1].Value;
                string output = groups[3].Value;

                instructions[output] = input;
            }
            var solution = getValue("a");
            return solution.ToString();
        }

        public override string getSolutionPart2()
        {
            instructions["b"] = instructionSolved["a"].ToString();
            instructionSolved.Clear();
            var solution = getValue("a");
            return solution.ToString();
        }
    }

    public class Day8 : Day
    {
        const int number = 8;
        public Day8() : base(number) { }

        private int getMemoryCharacters(string text)
        {
            text = text.Replace("\\\\", "1");
            text = Regex.Replace(text, @"\\x..", "1");
            text = text.Replace("\\\"", "1");
            return text.Length - 2;
        }

        private int getEncodedCharacters(string text)
        {
            text = text.Replace("\\", "\\\\");
            text = text.Replace("\"", "\\\"");
            return text.Length + 2;
        }
        public override string getSolutionPart1()
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

        public override string getSolutionPart2()
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

    public class Day9 : Day
    {
        const int number = 9;

        public Day9() : base(number) { }

        struct Route
        {
            public string city1;
            public string city2;
            public int distance;
        }

        List<Route> Routes = new List<Route>();
        HashSet<string> cities = new HashSet<string>();

        private int getDistance(string start, int distance, List<string> targets, bool longest = false)
        {
            if (targets.Count == 1)
            {
                return distance + Routes
                                    .Where(r => r.city1 == start && r.city2 == targets.Single() || r.city2 == start && r.city1 == targets.Single())
                                    .Select(r => r.distance)
                                    .Single();
            }

            List<int> distances = new List<int>();

            foreach (var target in targets)
            {
                distances.Add(
                    getDistance(
                        target,
                        Routes.Where(r => r.city1 == start && r.city2 == target || r.city2 == start && r.city1 == target).Select(r => r.distance).Single(),
                        targets.Where(t => t != target).ToList(),
                        longest
                        )
                   );
            }
            if (longest)
            {
                return distance + distances.Max();
            }
            else
            {
                return distance + distances.Min();
            }
            
        }

        public override string getSolutionPart1()
        {
            Regex regLine = new Regex(@"(\w+) to (\w+) = (\d+)");
            

            foreach (string line in inputLines)
            {
                var groups = regLine.Match(line).Groups;
                Route r = new Route
                {
                    city1 = groups[1].Value,
                    city2 = groups[2].Value,
                    distance = groups[3].Value.ToInt()
                };
                cities.Add(groups[1].Value);
                cities.Add(groups[2].Value);
                Routes.Add(r);
            }
            
            List<int> distances = new List<int>();
            foreach (var start in cities)
            {
                distances.Add(getDistance(start, 0, cities.Where(t => t != start).ToList()));
            }
            return distances.Min().ToString();
        }

        public override string getSolutionPart2()
        {
            List<int> distances = new List<int>();
            foreach (var start in cities)
            {
                distances.Add(getDistance(start, 0, cities.Where(t => t != start).ToList(), true));
            }
            return distances.Max().ToString();
        }
    }

    public class Day10 : Day
    {
        private const int number = 10;

        public Day10() : base(number) { }

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
