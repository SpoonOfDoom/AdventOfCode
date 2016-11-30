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
		private HashSet<string> startedSarches = new HashSet<string>();
        private int targetPresentCount = 36000000;
        private int lowestPossible;
		int lowestFound = int.MaxValue;

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

	    private bool FoundLowest(int candidate)
	    {
		    var checkedHouses = housePresents.Keys.Where(k => k >= lowestPossible && k <= candidate);
		    var availableHouses = Enumerable.Range(lowestPossible, candidate);
		    return Equals(checkedHouses, availableHouses);
	    }

	    private int GetLowestChecked()
	    {
		    var list = housePresents.Keys.ToList();
			list.Sort();
		    int result = lowestPossible;
		    for (int i = 1; i < list.Count; i++)
		    {
			    if (list[i-1] == list[i]-1)
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

		private int GetHighestChecked()
		{
			var list = housePresents.Keys.ToList();
			list.Sort();
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
            lowestPossible = currentStart;
            //currentStart = targetPresentCount/100;
            int maxPresentsFound = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            
	        int low = lowestPossible;
	        int high = targetPresentCount;

	        CalculateHousePresents(targetPresentCount);
	        var count = CalculateHousePresents(lowestPossible);
			
	        if (count >= targetPresentCount)
	        {
		        found = FoundLowest(lowestPossible);
	        }
	        int lowestChecked = GetLowestChecked();
	        int highestChecked = GetHighestChecked();

			while (!found)
	        {
		        int mid = (low + high)/2;
		        if (low == high-1 || low == high)
		        {
			        if (housePresents.ContainsKey(mid))
			        {
				        mid++;
				        if (housePresents.ContainsKey(mid))
				        {
					        low = lowestChecked + 1;
					        high = highestChecked - 1;
							mid = (low + high) / 2;
							int c1 = CalculateHousePresents(low);
							int c2 = CalculateHousePresents(high);
					        lowestChecked = GetLowestChecked();
					        highestChecked = GetHighestChecked();

					        if (c1 >= targetPresentCount && low < lowestFound)
					        {
						        lowestFound = low;
						        high = lowestFound;
						        low = lowestChecked;
						        found = FoundLowest(lowestFound);
						        continue;
					        }
					        if (c2 >= targetPresentCount && high < lowestFound)
					        {
						        lowestFound = high;
						        high = lowestFound;
						        low = lowestChecked;
						        found = FoundLowest(lowestFound);
						        continue;
					        }

				        }

					}

				}
		        int presentCount = CalculateHousePresents(mid);
				lowestChecked = GetLowestChecked();
				highestChecked = GetHighestChecked();
				if (presentCount >= targetPresentCount)
		        {
			        if (mid < lowestFound)
			        {
						highestChecked = GetHighestChecked();
						lowestFound = mid;
				        high = mid;
				        low = lowestChecked;
				        found = FoundLowest(lowestFound);
			        }
		        }
		        else
		        {
			        low = mid;
		        }
				
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
            var result = GetFirstTargetHouse();
            return result.ToString();
        }

        public override string GetSolutionPart2()
        {
            return base.GetSolutionPart2();
        }
    }
}