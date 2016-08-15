using System;
using System.Text.RegularExpressions;

namespace AdventOfCode.Days
{
    public class Day12 : Day
    {
        public Day12() : base(12) { }

        private static bool containsRed(dynamic json)
        {
            foreach (var child in json.Children())
            {
                if (child is Newtonsoft.Json.Linq.JProperty)
                {
                    var c = child as Newtonsoft.Json.Linq.JProperty;
                    if (c.Value is Newtonsoft.Json.Linq.JValue)
                    {
                        var val = c.Value as Newtonsoft.Json.Linq.JValue;
                        if (val.Value.ToString() == "red")
                        {
                            return true;
                        }
                    }
                }
                else if (child is Newtonsoft.Json.Linq.JValue)
                {
                    var c = child as Newtonsoft.Json.Linq.JValue;
                    if (c.Value.ToString() == "red")
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static int addNumbers(int sum, dynamic json)
        {
            int ret = 0;
            if (json is Newtonsoft.Json.Linq.JObject)
            {
                if (containsRed(json))
                {
                    return 0;
                }
                foreach (var child in json.Children())
                {
                    ret += addNumbers(sum, child);
                }

            }
            else if (json is Newtonsoft.Json.Linq.JArray)
            {
                var arr = json as Newtonsoft.Json.Linq.JArray;
                foreach (var item in arr)
                {
                    ret += addNumbers(sum, item);
                }
            }
            else if (json is Newtonsoft.Json.Linq.JProperty)
            {
                var prop = json as Newtonsoft.Json.Linq.JProperty;
                if (prop.Value is Newtonsoft.Json.Linq.JValue)
                {
                    var val = prop.Value as Newtonsoft.Json.Linq.JValue;
                    if (val.Type == Newtonsoft.Json.Linq.JTokenType.Integer)
                    {
                        int n = val.ToObject<int>();
                        ret += n;
                    }
                }

                if (prop.Value is Newtonsoft.Json.Linq.JObject || prop.Value is Newtonsoft.Json.Linq.JArray)
                {
                    if (!(prop.Value is Newtonsoft.Json.Linq.JObject && containsRed(prop.Value)))
                    {
                        foreach (var child in prop.Value.Children())
                        {
                            ret += addNumbers(sum, child);
                        }
                    }
                }
            }
            else if (json is Newtonsoft.Json.Linq.JValue)
            {
                var val = json as Newtonsoft.Json.Linq.JValue;
                if (val.Type == Newtonsoft.Json.Linq.JTokenType.Integer)
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

        public override string GetSolutionPart1()
        {
            Regex regNumbers = new Regex(@"(-?\d+)");

            var matches = regNumbers.Matches(input);
            int sum = 0;

            foreach (Match match in matches)
            {
                var number = int.Parse(match.Groups[1].Value);
                sum += number;
            }
            return sum.ToString();
        }
        
        public override string GetSolutionPart2()
        {
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(input);
            int sum = addNumbers(0, json);
            return sum.ToString();
        }
    }
}