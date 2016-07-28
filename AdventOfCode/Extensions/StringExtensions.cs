using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNumeric(this string s)
        {
            int a;
            return int.TryParse(s, out a);
        }

        public static int ToInt(this string s)
        {
            return int.Parse(s);
        }
    }
}
