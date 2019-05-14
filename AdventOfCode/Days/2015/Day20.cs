using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode.Days._2015
{
	// ReSharper disable once UnusedMember.Global
    public class Day20 : Day
    {
        public Day20() : base(2015, 20) { }

        private const int TargetPresentCount = 36000000;

        private static IEnumerable<int> GetDivisors(int number)
        {
	        List<int> divisors = new List<int>();
	        for (int i = 1; i <= Math.Sqrt(number); i++)
	        {
		        if (number%i == 0)
		        {
			        divisors.Add(i);
			        divisors.Add(number/i);
		        }
	        }
	        return divisors.Distinct();
        }


        private static long DoIt()
	    {
		    int presentCount;
		    
            long maxSoFar = int.MaxValue;

            Parallel.For(1,
	                     TargetPresentCount,
	                     (x) =>
	                     {
		                     if (x > maxSoFar)
	                         {
	                            return;
	                         }
	                         
                             IEnumerable<int> tempElves = GetDivisors(x);
                             presentCount = tempElves.Sum(e => e * 10);

                             if (presentCount >= TargetPresentCount)
	                         {
	                             if (x < maxSoFar)
	                             {
	                                 maxSoFar = x;
	                             }
	                         }
	                     });
	        return maxSoFar;
	    }

	    private static int DoItAgain()
	    {
		    var houses = new Dictionary<int, int>();
		    int maxPresents = int.MinValue;
			int target = TargetPresentCount;
			int presentsPerElf = 11;
			for (int i = 1; i < TargetPresentCount; i++)
			{
				var visitedHouses = Enumerable.Range(1, 50).Select(x => x*i).ToList();
				foreach (int house in visitedHouses)
				{
					if (!houses.ContainsKey(house))
					{
						houses[house] = 0;
					}
					houses[house] += i*presentsPerElf;
				}
				if (houses[i] >= target)
				{
					return i;
				}
				if (houses[i] > maxPresents)
				{
					maxPresents = houses[i];
				}
			}
			return -1;
		}

	    protected override object GetSolutionPart1()
        {
	        long result = DoIt();
			return result.ToString(); //831600
        }

        protected override object GetSolutionPart2()
        {
	        int result = DoItAgain();
            return result.ToString();
        }
    }
}
