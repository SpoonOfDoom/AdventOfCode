﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode.Days
{
    public abstract class Day
    {
#if DEBUG
        private const string Configuration = "Debug";
#else
        private const string Configuration = "Release";
#endif

        private const string TimeExportFolder = "exports";
        protected object solutionPart1, solutionPart2;
        protected TimeSpan solutionTime1, solutionTime2;
        private TimeSpan TotalTime => solutionTime1 + solutionTime2;
        protected string Input;
        protected List<string> InputLines;
        private readonly int year;
        private readonly int number;

        private static Dictionary<string, List<Dictionary<string, TimeSpan>>> solutionTimes = new Dictionary<string, List<Dictionary<string, TimeSpan>>>();

        private object SolutionPart1
        {
            get
            {
                if (solutionPart1 == null)
                {
                    GetSolutionPart1();
                }
                return solutionPart1;
            }
        }

        private object SolutionPart2
        {
            get
            {
                if (solutionPart2 == null)
                {
                    GetSolutionPart2();
                }
                return solutionPart2;
            }
        }

        protected Day(int year, int number)
        {
            this.year = year;
            this.number = number;
            GetInput();
        }

        /// <summary>
        /// Input will be entered in a separate method so that it can be collapsed individually (for bigger inputs)
        /// </summary>
        /// <returns></returns>
        private void GetInput()
        {
            Input = File.ReadAllText($"input\\{year}\\day{number:00}.txt");
            InputLines = File.ReadAllLines($"input\\{year}\\day{number:00}.txt").ToList();
        }

        protected virtual object GetSolutionPart1()
        {
            solutionPart1 = "not implemented.";
            solutionTime1 = TimeSpan.Zero;
            return SolutionPart1;
        }

        protected virtual object GetSolutionPart2()
        {
            solutionPart2 = "not implemented";
            solutionTime2 = TimeSpan.Zero;
            return SolutionPart2;
        }

        // ReSharper disable once UnusedMember.Global
        public static void RunAllDays(bool verbose = true)
        {
            var sw = new Stopwatch();
            sw.Start();
            for (int y = 2015; y <= 2018; y++) //todo: get existing years and days programatically
            {
                RunDaysOfYear(y, verbose: verbose, writeTimesOnFinish: false);
            }
            sw.Stop();

            WriteTimesToFile();
            Console.WriteLine($"Overall total time taken: {sw.Elapsed.Hours}:{sw.Elapsed.Minutes}:{sw.Elapsed.Seconds}:{sw.Elapsed.Milliseconds}");
            Console.ReadLine();
        }

        public static void RunDaysOfYear(int year, bool verbose = true, bool writeTimesOnFinish = true)
        {
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 1; i <= 25; i++) //todo: get existing years and days programatically
            {
                RunDay(year, i, batch: true, verbose: verbose);
            }
            sw.Stop();

            if (writeTimesOnFinish)
            {
                WriteTimesToFile();
            }
            Console.WriteLine($"Year {year} total time taken: {sw.Elapsed.Hours}:{sw.Elapsed.Minutes}:{sw.Elapsed.Seconds}:{sw.Elapsed.Milliseconds}");
            Console.ReadLine();
        }
        

        private static void WriteTimesToFile(string filename = "solutionTimes")
        {
            if (!Directory.Exists(TimeExportFolder))
            {
                Directory.CreateDirectory(TimeExportFolder);
            }
            if (filename == "solutionTimes")
            {
                filename += $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.csv";
            }
            string filePath = $"{TimeExportFolder}\\{filename}";
            string fileContent = "Machine Name;Build Configuration;Year;Day Number;Part 1;Part 2;Total\n";


            foreach (KeyValuePair<string, List<Dictionary<string, TimeSpan>>> solutionTime in solutionTimes)
            {
                foreach (Dictionary<string, TimeSpan> uniqueRun in solutionTime.Value)
                {
                    //not very pretty, but solutionTime.Key is a string of format {year};{day}, so there's one more semicolon here than we can see. Was the easiest day to adjust for multiple years.
                    //todo: make this less hacky.
                    fileContent += $"{Environment.MachineName};{Configuration};{solutionTime.Key};{uniqueRun["Part1"]};{uniqueRun["Part2"]};{uniqueRun["Total"]}\n";
                }
            }
            File.WriteAllText(filePath, fileContent, Encoding.UTF8);
        }

        private void WriteToFile(int part, bool append = true)
        {
            if (!Directory.Exists(TimeExportFolder))
            {
                Directory.CreateDirectory(TimeExportFolder);
            }
            string filename = $"year_{year}_day_{number:00}.log";

            string filePath = $"{TimeExportFolder}\\{filename}";

            string solution = part == 1 ? SolutionPart1.ToString() : SolutionPart2.ToString();
            TimeSpan solutionTime = part == 1 ? solutionTime1 : solutionTime2;

            string fileContent = $"{Environment.MachineName}|{Configuration}\\Year {year}, Day {number:00} - Part {part}: {solution} (solved in {solutionTime.TotalSeconds} seconds / {solutionTime}, saved at {DateTime.Now:yyyy-MM-dd_HH-mm-ss})\n";

            if (append)
            {
                File.AppendAllText(filePath, fileContent, Encoding.UTF8);
            }
            else
            {
                File.WriteAllText(filePath, fileContent, Encoding.UTF8);
            }
        }

        public static void RunDay(int year, int number, Day dayInstance = null, bool batch = false, bool verbose = true, int times = 1)
        {
            if (dayInstance == null)
            {
                Type dayType = Type.GetType($"AdventOfCode.Days._{year}.Day{number:00}");
                if (dayType == null)
                {
                    throw new Exception($"Couldn't find type AdventOfCode.Days._{year}.Day{number:00}");
                }
                dayInstance = (Day)Activator.CreateInstance(dayType);
            }


            for (int i = 0; i < times; i++)
            {
                var sw = new Stopwatch();

                Console.WriteLine($"Starting {year}, day {number}, part 1 at {DateTime.Now}...");
                sw.Start();
                object solution1 = dayInstance.GetSolutionPart1();
                sw.Stop();
                if (dayInstance.solutionPart1 == null)
                {
                    dayInstance.solutionPart1 = solution1;
                    dayInstance.solutionTime1 = sw.Elapsed;
                }


                //dayInstance.WriteToFile();
                if (verbose)
                {
                    Console.WriteLine($"year {dayInstance.year}, day {dayInstance.number:00} part 1 : {dayInstance.SolutionPart1} - solved in {dayInstance.solutionTime1.TotalSeconds} seconds ({dayInstance.solutionTime1.TotalMilliseconds} milliseconds)");
                }
                try
                {
                    dayInstance.WriteToFile(1);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                Console.WriteLine($"Starting {year}, day {number}, part 2 at {DateTime.Now}...");
                sw.Restart();
                object solution2 = dayInstance.GetSolutionPart2();
                sw.Stop();

                if (dayInstance.solutionPart2 == null)
                {
                    dayInstance.solutionPart2 = solution2;
                    dayInstance.solutionTime2 = sw.Elapsed;
                }


                if (verbose)
                {
                    Console.WriteLine($"year {dayInstance.year}, day {dayInstance.number:00} part 2 : {dayInstance.SolutionPart2} - solved in {dayInstance.solutionTime2.TotalSeconds} seconds ({dayInstance.solutionTime2.TotalMilliseconds} milliseconds)");
                    Console.WriteLine($"total time: {dayInstance.TotalTime.TotalSeconds} seconds ({dayInstance.TotalTime.TotalMilliseconds} milliseconds)");
                }

                try
                {
                    dayInstance.WriteToFile(2);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                var run = new Dictionary<string, TimeSpan>
                {
                    {"Total", dayInstance.TotalTime},
                    {"Part1", dayInstance.solutionTime1},
                    {"Part2", dayInstance.solutionTime2}
                };

                string key = $"{year};{number}";
                if (!solutionTimes.ContainsKey(key))
                {
                    solutionTimes[key] = new List<Dictionary<string, TimeSpan>>();
                }
                solutionTimes[key].Add(run);
            }

            if (!batch)
            {
                Console.Read();
            }
        }
    }
}
