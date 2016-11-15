using System;
using AdventOfCode.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace AdventOfCode.Days
{
    public class Day18 : Day
    {
        public Day18() : base(18) { }
        
        private bool[,] grid = new bool[100, 100];

        private void PopulateInitialGrid()
        {
            for (int i = 0; i < inputLines.Count; i++)
            {
                string line = inputLines[i];
                for (int j = 0; j < line.Length; j++)
                {
                    if (line[j] == '#')
                    {
                        grid[i, j] = true;
                    }
                    else
                    {
                        grid[i, j] = false;
                    }
                }
            }
        }

        private List<bool> GetNeighbours(bool[,] state, int x, int y)
        {
            List<bool> neighbours = new List<bool>();

            //assuming the given x and y values are inside the grid size

            //left column
            if (x - 1 >= 0)
            {
                if (y - 1 >= 0)
                {
                    neighbours.Add(state[x - 1, y - 1]);
                }
                neighbours.Add(state[x - 1, y]);

                if (y + 1 < 100)
                {
                    neighbours.Add(state[x - 1, y + 1]);
                }
            }

            //middle column
            if (y - 1 >= 0)
            {
                neighbours.Add(state[x, y - 1]);
            }
            if (y + 1 < 100)
            {
                neighbours.Add(state[x, y + 1]);
            }

            //right column
            if (x + 1 < 100)
            {
                if (y - 1 >= 0)
                {
                    neighbours.Add(state[x + 1, y - 1]);
                }

                neighbours.Add(state[x + 1, y]);

                if (y + 1 < 100)
                {
                    neighbours.Add(state[x + 1, y + 1]);
                }
            }

            return neighbours;
        }

        private bool ShouldLightBeOn(bool[,] state, int x, int y)
        {
            //The state a light should have next is based on its current state (on or off) plus the number of neighbors that are on:

            //A light which is on stays on when 2 or 3 neighbors are on, and turns off otherwise.
            //A light which is off turns on if exactly 3 neighbors are on, and stays off otherwise.

            

            if (state[x,y] == true)
            {
                var neighbours = GetNeighbours(state, x, y).Where(n => n == true);
                return neighbours.Count() == 2 || neighbours.Count() == 3;
            }
            else
            {
                var neighbours = GetNeighbours(state, x, y).Where(n => n == true);
                return neighbours.Count() == 3;
            }
        }

        private bool[,] CalculateNewState(bool[,] oldState)
        {
            bool [,] newGrid = new bool[100,100];

            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 100; y++)
                {
                    newGrid[x, y] = ShouldLightBeOn(oldState, x, y);
                }
            }
            return newGrid;
        }

        private void PrintGrid(bool[,] grid)
        {
            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 100; y++)
                {
                    string symbol;
                    if (grid[x,y])
                    {
                        symbol = "#";
                    }
                    else
                    {
                        symbol = ".";
                    }
                    Console.Write(symbol);
                }
                Console.Write("\n");
            }
        }

        public override string GetSolutionPart1()
        {
            PopulateInitialGrid();
            for (int i = 0; i < 100; i++)
            {
                grid = CalculateNewState(grid);
                PrintGrid(grid);
                Thread.Sleep(500);
            }
            //In your grid of 100x100 lights, given your initial configuration, how many lights are on after 100 steps?
            int count = 0;
            foreach (bool b in grid)
            {
                if (b == true)
                {
                    count++;
                }
            }
            return count.ToString();
        }

        public override string GetSolutionPart2()
        {
            throw new NotImplementedException();
        }
    }
}