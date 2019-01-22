using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Days._2015
{
    // ReSharper disable once UnusedMember.Global
    public class Day18 : Day
    {
        public Day18() : base(2015, 18) { }
        

        private bool[,] PopulateInitialGrid(bool cornersOn = false)
        {
            var grid = new bool[100,100];
            for (int i = 0; i < InputLines.Count; i++)
            {
                string line = InputLines[i];
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
            if (cornersOn)
            {
                grid[0, 0] = true;
                grid[0, 99] = true;
                grid[99, 0] = true;
                grid[99, 99] = true;
            }
            return grid;
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

        private bool[,] CalculateNewState(bool[,] oldState, bool cornersOn = false)
        {
            bool [,] newGrid = new bool[100,100];

            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 100; y++)
                {
                    newGrid[x, y] = ShouldLightBeOn(oldState, x, y);
                }
            }

            if (cornersOn)
            {
                newGrid[0, 0] = true;
                newGrid[0, 99] = true;
                newGrid[99, 0] = true;
                newGrid[99, 99] = true;
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

        protected override object GetSolutionPart1()
        {
            var grid = PopulateInitialGrid();
            for (int i = 0; i < 100; i++)
            {
                grid = CalculateNewState(grid);
            }

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

        protected override object GetSolutionPart2()
        {
            var grid = PopulateInitialGrid(cornersOn: true);
            
            for (int i = 0; i < 100; i++)
            {
                grid = CalculateNewState(grid, cornersOn:true);
            }

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
    }
}
