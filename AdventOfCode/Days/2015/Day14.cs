using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions;

namespace AdventOfCode.Days._2015
{
    // ReSharper disable once UnusedMember.Global
    public class Day14 : Day
    {
        public Day14() : base(2015, 14) { }

        class Reindeer
        {
#pragma warning disable 414
            public string Name;
#pragma warning restore 414
            public int Speed;
            public int RunTime;
            public int RestTime;
            public int Distance;
            public int Score;

            public bool IsResting;
            public int Second;

            public int DistanceRun(int time)
            {
                int distance = 0;

                while (time > 0)
                {
                    distance += Speed * Math.Min(RunTime, time);
                    time -= RunTime;

                    time -= RestTime;
                }
                return distance;
            }

            public void Iterate()
            {
                if (IsResting)
                {
                    Second++;
                    if (Second >= RestTime)
                    {
                        IsResting = false;
                        Second = 0;
                    }
                }
                else
                {
                    Second++;
                    Distance += Speed;
                    if (Second >= RunTime)
                    {
                        IsResting = true;
                        Second = 0;
                    }
                }
            }
        }

        readonly Regex regLine = new Regex(@"(\w+) .+ (\d+) km\/s for (\d+) seconds.+ (\d+)");
        private List<Reindeer> reindeers = new List<Reindeer>();
        private const int raceTime = 2503;

        private Reindeer ParseLine(string line)
        {
            var groups = regLine.Match(line).Groups;

            return new Reindeer()
            {
                Name = groups[1].Value,
                Speed = groups[2].Value.ToInt(),
                RunTime = groups[3].Value.ToInt(),
                RestTime = groups[4].Value.ToInt(),
                Score = 0,
                Distance = 0,
                IsResting = false,
                Second = 0
            };
        }

        private static void IterateList(List<Reindeer> reindeerList)
        {
            foreach (Reindeer reindeer in reindeerList)
            {
                reindeer.Iterate();
            }
            var d = reindeerList.Max(reindeer => reindeer.Distance);
            foreach (var reindeer in reindeerList.Where(r => r.Distance == d))
            {
                reindeer.Score++;
            }
        }

        protected override object GetSolutionPart1()
        {
            foreach (string line in InputLines)
            {
                reindeers.Add(ParseLine(line));
            }
            

            var biggestDistance = reindeers.Select(r => r.DistanceRun(raceTime)).Max();
            return biggestDistance.ToString();
        }

        protected override object GetSolutionPart2()
        {
            for (int i = 0; i < raceTime; i++)
            {
                IterateList(reindeers);
            }

            int winningScore = reindeers.Max(r => r.Score);
            return winningScore.ToString();
        }
    }
}
