using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day20 : Day
    {
        public Day20() : base(20) { }

        private Dictionary<int,int> housePresents = new Dictionary<int, int>();
        private int targetPresentCount = 36000000;

        private static bool IsDivisible(int number, int divider)
        {
            return number%divider == 0;
        }

        private static IEnumerable<int> GetDivisors(int number)
        {
            var candidates = Enumerable.Range(1, number);
            return candidates.Where(x => IsDivisible(number, x));
        }

        private int CalculateHousePresents(int houseNumber)
        {
            var divisors = GetDivisors(houseNumber);
            int presents = divisors.Sum(divisor => divisor*10);
            housePresents[houseNumber] = presents;

            return presents;
        }

        private static int GetRangeSum(int number)
        {
            var numbers = Enumerable.Range(0, number);
            return numbers.Sum(i => i*10);
        }

        private int GetFirstTargetHouse()
        {
            int currentStart = 1;
            const int batchSize = 1000;
            bool found = false;

            for (int i = 0; i < targetPresentCount; i++)
            {
                int rangeSum = GetRangeSum(i);
                if (rangeSum >= targetPresentCount)
                {
                    currentStart = i;
                    break;
                }
            }
            currentStart = targetPresentCount/100;
            int maxPresentsFound = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (!found)
            {
                Console.Clear();
                Console.WriteLine("Current start: {0}", currentStart);
                var candidates = Enumerable.Range(currentStart, batchSize);
                Console.WriteLine("Max presents found: {0}", maxPresentsFound);
                Console.WriteLine("Difference to target: {0}", targetPresentCount - maxPresentsFound);
                Console.WriteLine("Time: {0}", sw.Elapsed);
                Parallel.ForEach(candidates, house =>
                {
                    int presents = CalculateHousePresents(house);
                    maxPresentsFound = Math.Max(presents, maxPresentsFound);

                    if (presents >= targetPresentCount)
                    {
                        found = true;
                    }
                });

                currentStart += batchSize;
            }
            return -1;
        }
        
        public override string GetSolutionPart1()
        {
            var result = GetFirstTargetHouse();
            return result.ToString();
        }

        public override string GetSolutionPart2()
        {
            return base.GetSolutionPart2();
        }
    }
}