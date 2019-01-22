using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdventOfCode.Days._2015
{
    // ReSharper disable once UnusedMember.Global
    public class Day12 : Day
    {
        public Day12() : base(2015, 12) { }

        private static bool ContainsRed(dynamic json)
        {
            foreach (var child in json.Children())
            {
                if (child is JProperty)
                {
                    var c = child as JProperty;
                    if (c.Value is JValue)
                    {
                        var val = c.Value as JValue;
                        if (val.Value.ToString() == "red")
                        {
                            return true;
                        }
                    }
                }
                else if (child is JValue)
                {
                    var c = child as JValue;
                    if (c.Value.ToString() == "red")
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static int AddNumbers(int sum, dynamic json)
        {
            int ret = 0;
            if (json is JObject)
            {
                if (ContainsRed(json))
                {
                    return 0;
                }
                foreach (var child in json.Children())
                {
                    ret += AddNumbers(sum, child);
                }

            }
            else if (json is JArray)
            {
                var arr = json as JArray;
                foreach (var item in arr)
                {
                    ret += AddNumbers(sum, item);
                }
            }
            else if (json is JProperty)
            {
                var prop = json as JProperty;
                if (prop.Value is JValue)
                {
                    var val = prop.Value as JValue;
                    if (val.Type == JTokenType.Integer)
                    {
                        int n = val.ToObject<int>();
                        ret += n;
                    }
                }

                if (prop.Value is JObject || prop.Value is JArray)
                {
                    if (!(prop.Value is JObject && ContainsRed(prop.Value)))
                    {
                        foreach (var child in prop.Value.Children())
                        {
                            ret += AddNumbers(sum, child);
                        }
                    }
                }
            }
            else if (json is JValue)
            {
                var val = json as JValue;
                if (val.Type == JTokenType.Integer)
                {
                    int n = val.ToObject<int>();
                    ret += n;
                }
            }
            else
            {
                Console.WriteLine("Something!");
            }
            return sum + ret;
        }

        protected override object GetSolutionPart1()
        {
            Regex regNumbers = new Regex(@"(-?\d+)");

            var matches = regNumbers.Matches(Input);
            int sum = 0;

            foreach (Match match in matches)
            {
                var number = int.Parse(match.Groups[1].Value);
                sum += number;
            }
            return sum.ToString();
        }
        
        protected override object GetSolutionPart2()
        {
            dynamic json = JsonConvert.DeserializeObject(Input);
            int sum = AddNumbers(0, json);
            return sum.ToString();
        }
    }
}
