using AdventOfCode.days;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    class Program
    {
        

        static void Main(string[] args)
        {
            day d;

            try
            {
                d = new Day4();
            }
            catch (InputEmptyException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to exit...");
                Console.Read();
                return;
            }
            

            string solution1, solution2 = "";

            try
            {
                solution1 = d.getSolutionPart1().ToString();
            }
            catch (NotImplementedException)
            {
                solution1 = "not implemented.";
            }
            Console.WriteLine("day " + d.Number + " part 1 : " + solution1);

            try
            {
                solution2 = d.getSolutionPart2().ToString();
            }
            catch (NotImplementedException)
            {
                solution2 = "not implemented.";
            }
            Console.WriteLine("day " + d.Number + " part 2 : " + solution2);

            Console.Read();
        }
    }
}
