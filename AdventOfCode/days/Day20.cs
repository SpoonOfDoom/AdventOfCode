using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day20 : Day
    {
        public Day20() : base(20) { }

        private static Dictionary<int,int> housePresents = new Dictionary<int, int>();
		private static HashSet<string> startedSarches = new HashSet<string>();
	    private const int targetPresentCount = 36000000;
	    private static int lowestPossible;
	    private static int lowestChecked = int.MinValue + 1;
	    private static int highestChecked = int.MaxValue - 1;
		private static int lowestFound = int.MaxValue;
		private static IEnumerable<int> uncheckedNumbers;

		private static bool IsDivisible(int number, int divider)
        {
            return number%divider == 0;
        }

        private static IEnumerable<int> GetDivisors(int number)
        {
            return Enumerable.Range(1, number).Where(x => number%x == 0);
        }


        private static int CalculateHousePresents(int houseNumber)
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

        private bool SearchHouse(int low, int high, ref int lowestFound, int level = 0)
        {
	        string searchString = $"{low}-{high}";
	        if (startedSarches.Contains(searchString))
	        {
		        return false;
	        }
	        else
	        {
		        startedSarches.Add(searchString);
	        }
			Console.Clear();
	        Console.WriteLine("Level: {0}", level);
			Console.WriteLine("Lowest found: {0}", lowestFound);
	        Console.WriteLine("Numbers checked: {0}", housePresents.Count);
	        Console.WriteLine("Searches started: {0}", startedSarches.Count);
            if (lowestFound < low)
            {
                return false;
            }
			if (lowestFound < high)
			{
				high = lowestFound;
			}
			int mid = (low + high)/2;
            if (lowestFound < mid)
            {
				return SearchHouse(low, lowestFound, ref lowestFound, level + 1);
            }
            if (housePresents.ContainsKey(mid))
            {
                 //dirty, dirty, temporary hack
                if (high == low+1)
                {
                    mid = mid + 1;
                    if (housePresents.ContainsKey(mid))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
                
            }
            
            int presentCount = CalculateHousePresents(mid);
            if (presentCount >= targetPresentCount)
            {
	            lowestFound = Math.Min(lowestFound, mid);

				return SearchHouse(lowestPossible, Math.Min(mid, lowestFound), ref lowestFound, level + 1);
            }
            else
            {
                if (lowestFound < int.MaxValue)
                {
	                if (mid < lowestFound)
	                {
						SearchHouse(low, mid, ref lowestFound, level + 1);
		                if (mid < lowestFound)
		                {
							return SearchHouse(mid, lowestFound, ref lowestFound, level + 1);
						}
					}
	                else
	                {
						return SearchHouse(low, mid, ref lowestFound, level + 1);
					}
                }
                else
                {
	                if (mid < lowestFound)
	                {
						SearchHouse(mid, Math.Min(high, lowestFound), ref lowestFound, level + 1);
					}
	                return SearchHouse(low, mid, ref lowestFound, level + 1);
                }
            }
            return true;
        }

	    private static bool FoundLowest(int candidate)
	    {
		    var checkedHouses = housePresents.Keys.Where(k => k >= lowestPossible && k <= candidate);
		    var availableHouses = Enumerable.Range(lowestPossible, candidate);
		    return Equals(checkedHouses, availableHouses);
	    }

	    private static int GetLowestChecked()
	    {
		    var list = housePresents.Keys.ToList();
			list.Sort();
		    var numbers = Enumerable.Range(Math.Max(lowestChecked, 1), Math.Min(lowestFound, targetPresentCount));
		    return numbers.Except(list).First() -1;
		    //int result = lowestPossible;
		    //for (int i = 1; i < list.Count; i++)
		    //{
			   // if (list[i-1] == list[i]-1)
			   // {
				  //  result = list[i];
			   // }
			   // else
			   // {
				  //  return result;
			   // }
		    //}
		    //return result;
	    }

		private static int GetHighestChecked()
		{
			var list = housePresents.Keys.ToList();
			list.Sort();
			//var numbers = Enumerable.Range(Math.Max(0, lowestFound), Math.Min(highestChecked, targetPresentCount));
			//var test = numbers.Except(list).ToList();
			//var lastNumber = test.Max();
			int result = lowestFound;
			int start = list.IndexOf(lowestFound);
			for (int i = start - 1; i >= 0; i--)
			{
				if (list[i + 1] == list[i] + 1)
				{
					result = list[i];
				}
				else
				{
					return result;
				}
			}
			return result;
		}

	    private static IEnumerable<int> GetUncheckedNumbers()
	    {
		    var numbers = Enumerable.Range(lowestChecked, highestChecked);
		    var checkedNumbers = housePresents.Keys.ToList();
		    return numbers.Except(checkedNumbers);
	    }

		private static bool CheckHighOnly(ref int low, ref int high, out int lowestChecked, out int highestChecked, ref bool found)
		{
			int c2 = CalculateHousePresents(high);
			lowestChecked = GetLowestChecked();
			highestChecked = GetHighestChecked();
			
			if (c2 >= targetPresentCount && high < lowestFound)
			{
				lowestFound = high;
				high = lowestFound;
				low = lowestChecked;
				found = FoundLowest(lowestFound);
				return true;
			}
			return false;
		}

		private static bool CheckLowOnly(ref int low, ref int high, out int lowestChecked, out int highestChecked, ref bool found)
		{
			int c1 = CalculateHousePresents(low);
			lowestChecked = GetLowestChecked();
			highestChecked = GetHighestChecked();

			if (c1 >= targetPresentCount && low < lowestFound)
			{
				lowestFound = low;
				high = lowestFound;
				low = lowestChecked;
				found = FoundLowest(lowestFound);
				return true;
			}
			return false;
		}

		private static bool CheckHighAndLow(ref int low, ref int high, ref bool found)
	    {
			int c1 = housePresents.ContainsKey(low) ? -1 : CalculateHousePresents(low);
			int c2 = housePresents.ContainsKey(high) ? -1 : CalculateHousePresents(high);

		    if (c1 != -1 || c2 != -1)
		    {
				lowestChecked = GetLowestChecked();
				highestChecked = GetHighestChecked();


				if (c1 != -1 && (c1 >= targetPresentCount && low < lowestFound))
				{
					lowestFound = low;
					high = lowestFound;
					low = lowestChecked;
					found = FoundLowest(lowestFound);
					return true;
				}
				if (c2 != -1 && (c2 >= targetPresentCount && high < lowestFound))
				{
					lowestFound = high;
					high = lowestFound;
					low = lowestChecked;
					found = FoundLowest(lowestFound);
					return true;
				}
			}
			
		    return false;
	    }

	    private static int DoIt()
	    {
		    int presentCount = 0;
		    
            int maxSoFar = int.MaxValue;
            object lockObject = new object();
	        int amountChecked = 0;
	        Parallel.For(1,
	                     targetPresentCount,
	                     (x) =>
	                     {
	                         amountChecked++;
	                         lock (lockObject)
	                         {
	                             if (x >= maxSoFar)
	                             {
	                                 return;
	                             }
	                         }
                             IEnumerable<int> tempElves = GetDivisors(x);
                             presentCount = tempElves.Sum(e => e * 10);

                             if (presentCount >= targetPresentCount)
                             {
                                 lock (lockObject)
                                 {
                                     if (x < maxSoFar)
                                     {
                                         maxSoFar = x;
                                     }
                                 }
                             }
                         });
	        return maxSoFar;
	    }

	    private static int DoItAgain()
	    {
		    var houses = new Dictionary<int, int>();
		    int maxPresents = int.MinValue;
			int target = targetPresentCount;
			int presentsPerElf = 11;
			for (int i = 1; i < targetPresentCount; i++)
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
				Console.Write($"i: {i} - houses count: {houses.Count} - max present count: {maxPresents}\r");
			}
			return -1;
		}

		private static int GetFirstTargetHouse()
        {
            var currentStart = 1;
            var found = false;

            for (int i = 0; i < targetPresentCount; i++)
            {
                int rangeSum = GetRangeSum(i);
                if (rangeSum >= targetPresentCount)
                {
                    currentStart = i;
                    break;
                }
            }
            lowestPossible = currentStart;
	        lowestChecked = lowestPossible;
	        highestChecked = targetPresentCount;
            //currentStart = targetPresentCount/100;
            //int maxPresentsFound = 0;
            var sw = new Stopwatch();
            sw.Start();
            
	        int low = lowestPossible;
	        int high = targetPresentCount;

	        CheckHighAndLow(ref low, ref high, ref found);
	        var count = CalculateHousePresents(lowestPossible);
			
	        if (count >= targetPresentCount)
	        {
		        found = FoundLowest(lowestPossible);
	        }
	        lowestChecked = GetLowestChecked();
	        highestChecked = GetHighestChecked();
	        int mid, c1, c2, presentCount;
	        TimeSpan? timeTo1000 = null;
	        TimeSpan? timeTo10000 = null;
	        bool highLow = false;
	        int helpInt = 0;
			while (!found)
	        {
				mid = (low + high)/2;
		        if (low == high-1 || low == high)
		        {
			        if (housePresents.ContainsKey(mid))
			        {
				        mid++;
					}
				}
				if (housePresents.ContainsKey(mid))
				{
					if (highLow)
					{
						high = highestChecked - 1;
					}
					else
					{
						low = lowestChecked + 1;
					}
					highLow = !highLow;
					mid = (low + high) / 2;
					if (CheckHighAndLow(ref low, ref high, ref found))
					{
						continue;
					}
				}
				presentCount = CalculateHousePresents(mid);
				lowestChecked = GetLowestChecked();
				highestChecked = GetHighestChecked();
				if (presentCount >= targetPresentCount)
		        {
			        if (mid < lowestFound)
			        {
						highestChecked = GetHighestChecked();
						lowestFound = mid;
				        high = mid-1;
				        low = lowestChecked+1;
				        CheckHighAndLow(ref low, ref high, ref found);
				        found = FoundLowest(lowestFound);
			        }
			        else
			        {
				        Console.WriteLine("");
			        }
		        }
		        else
		        {
			        low = mid+1;
					CheckHighAndLow(ref low, ref high, ref found);
				}
				//CheckHighAndLow(ref low, ref high, out lowestChecked, out highestChecked, ref found);
				
				Console.Clear();
		        Console.WriteLine("Lowest found: {0}", lowestFound);
		        Console.WriteLine("Lowest checked: {0}", lowestChecked);
		        Console.WriteLine("Highest checked: {0}", highestChecked);
		        Console.WriteLine("Checked numbers: {0}", housePresents.Count);
		        Console.WriteLine("Time: {0}", sw.Elapsed);
		        if (housePresents.Count >= 1000 && !timeTo1000.HasValue)
		        {
					timeTo1000 = sw.Elapsed;
				}
				if (housePresents.Count >= 10000 && !timeTo10000.HasValue)
		        {
					timeTo10000 = sw.Elapsed;
				}
		        Console.WriteLine("Time to 1000: {0}", timeTo1000);
		        Console.WriteLine("Time to 10000: {0}", timeTo10000);
		        if (lowestFound < 2300000 && helpInt == 0)
		        {
					uncheckedNumbers = GetUncheckedNumbers();
				}
		  //      if (uncheckedNumbers != null)
		  //      {
				//	Console.WriteLine("Unchecked numbers between lowest and highest: {0}", uncheckedNumbers.Count());
				//}
		        helpInt++;
		        helpInt = helpInt%100;
	        }
			//found = SearchHouse(currentStart, targetPresentCount, ref lowestFound);
			//found = SearchHouse(currentStart, lowestFound, ref lowestFound);
			//if (found)
   //         {
   //             return housePresents.Where(x => x.Value >= targetPresentCount).Select(x => x.Key).Min();
   //         }

            //while (!found)
            //{
            //    Console.Clear();
            //    Console.WriteLine("Current start: {0}", currentStart);
            //    var candidates = Enumerable.Range(currentStart, batchSize);
            //    Console.WriteLine("Max presents found: {0}", maxPresentsFound);
            //    Console.WriteLine("Difference to target: {0}", targetPresentCount - maxPresentsFound);
            //    Console.WriteLine("Time: {0}", sw.Elapsed);
            //    Parallel.ForEach(candidates, house =>
            //    {
            //        int presents = CalculateHousePresents(house);
            //        maxPresentsFound = Math.Max(presents, maxPresentsFound);

            //        if (presents >= targetPresentCount)
            //        {
            //            found = true;
            //        }
            //    });

            //    currentStart += batchSize;
            //}
            return lowestFound;
        }
        
        public override string GetSolutionPart1()
        {
	        //return "Skipping this - solution was: 831600";
			int result = DoIt();
			return result.ToString(); //831600
        }

        public override string GetSolutionPart2()
        {
	        int result = DoItAgain();
            return result.ToString();
        }
    }
}