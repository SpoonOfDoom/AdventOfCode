using AdventOfCode.days;
using System;
using System.Diagnostics;

namespace AdventOfCode
{
    class Program
    {
        

        static void Main(string[] args)
        {
            Day d;
            Stopwatch sw = new Stopwatch();
            try
            {
                d = new Day13();
            }
            catch (InputEmptyException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to exit...");
                Console.Read();
                return;
            }
            

            string solution1, solution2 = "";
            TimeSpan totalTime = new TimeSpan();
            try
            {
                sw.Start();
                solution1 = d.getSolutionPart1().ToString();
                sw.Stop();
                totalTime += sw.Elapsed;
            }
            catch (NotImplementedException)
            {
                solution1 = "not implemented.";
            }
            Console.WriteLine($"day {d.Number} part 1 : {solution1} - solved in {sw.Elapsed.TotalSeconds} seconds ({sw.Elapsed.TotalMilliseconds} milliseconds)");

            try
            {
                sw.Restart();
                solution2 = d.getSolutionPart2().ToString();
                sw.Stop();
                totalTime += sw.Elapsed;
            }
            catch (NotImplementedException)
            {
                solution2 = "not implemented.";
            }
            Console.WriteLine($"day {d.Number} part 2 : {solution2} - solved in {sw.Elapsed.TotalSeconds} seconds ({sw.Elapsed.TotalMilliseconds} milliseconds)");
            Console.WriteLine($"total time: {totalTime.TotalSeconds} seconds ({totalTime.TotalMilliseconds} milliseconds)");

            Console.Read();
        }
    }
}
