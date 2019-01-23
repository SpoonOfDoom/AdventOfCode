using System;
using System.Collections.Generic;
using AdventOfCode.Extensions;
using AdventOfCode2017.Tools.FrequentlyUsedEnums;

namespace AdventOfCode.Days._2017
{
    // ReSharper disable once UnusedMember.Global
    class Day03 : Day
	{
		public Day03() : base(2017, 3) {}

	    private int GetMinimumDimension(int targetNumber)
	    {
	        double root = Math.Sqrt(targetNumber);
	        int minimumDimension = (int) Math.Ceiling(root);
	        if (minimumDimension % 2 == 0)
	        {
	            minimumDimension++; //grids with a "center" are much easier to handle, and one more row and column won't kill us.
	        }
	        return minimumDimension;
	    }

		private Tuple<int, int> GetNeighbourCoords(Tuple<int, int> start, Direction direction, int xRightScale = 1, int yUpScale = -1)
		{
			Tuple<int, int> neighbour;
			switch (direction)
			{
				case Direction.Up:
					neighbour = new Tuple<int, int>(start.Item1, start.Item2 + yUpScale);
					break;
				case Direction.Down:
					neighbour = new Tuple<int, int>(start.Item1, start.Item2 - yUpScale);
					break;
				case Direction.Left:
					neighbour = new Tuple<int, int>(start.Item1 - xRightScale, start.Item2);
					break;
				case Direction.Right:
					neighbour = new Tuple<int, int>(start.Item1 + xRightScale, start.Item2);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
			}

			return neighbour;
		}
		
		private int GetNeighbourValue(Dictionary<Tuple<int, int>, int> grid, Tuple<int, int> start, Direction direction, int xRightScale = 1, int yUpScale = -1)
		{
			Tuple<int, int> neighbour;
			switch (direction)
			{
				case Direction.Up:
					neighbour = new Tuple<int, int>(start.Item1, start.Item2 + yUpScale);
					break;
				case Direction.Down:
					neighbour = new Tuple<int, int>(start.Item1, start.Item2 - yUpScale);
					break;
				case Direction.Left:
					neighbour = new Tuple<int, int>(start.Item1 - xRightScale, start.Item2);
					break;
				case Direction.Right:
					neighbour = new Tuple<int, int>(start.Item1 + xRightScale, start.Item2);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
			}

			return grid.ContainsKey(neighbour) ? grid[neighbour] : 0;
			
		}

		private Direction GetNextDirection(Dictionary<Tuple<int, int>, int> grid, Tuple<int, int> currentPosition, Direction currentDirection)
		{
			if (currentPosition.Equals(new Tuple<int, int>(0,0)))
			{
				return Direction.Right;
			}

			Direction turnedDirection = currentDirection.TurnLeft();
			Tuple<int, int> turnedPosition = GetNeighbourCoords(currentPosition, turnedDirection);
			return grid.ContainsKey(turnedPosition) ? currentDirection : turnedDirection;
		}

	    private Tuple<int, int> CreateAndFillGrid(int targetNumber)
	    {
		    Dictionary<Tuple<int, int>, int> grid = new Dictionary<Tuple<int, int>, int>();
		    Tuple<int, int> position = new Tuple<int, int>(0, 0);
		    Direction direction = Direction.Right;
		    for (int i = 1; i <= targetNumber; i++)
		    {
			    if (i == 1)
			    {
				    grid[position] = i;
				    continue;
			    }
			    direction = GetNextDirection(grid, position, direction);
			    position = GetNeighbourCoords(position, direction);
			    grid[position] = i;
//			    PaintGrid(grid, targetNumber);
		    }
		    
		    return position;
	    }
		
		private int CreateAndFillSumGrid(int targetNumber)
		{
			Dictionary<Tuple<int, int>, int> grid = new Dictionary<Tuple<int, int>, int>();
			Tuple<int, int> position = new Tuple<int, int>(0, 0);
			Direction direction = Direction.Right;
			int value = 1;
			bool start = true;
			while (value <= targetNumber)
			{
				if (start)
				{
					grid[position] = value;
					start = false;
					continue;
				}
				direction = GetNextDirection(grid, position, direction);
				position = GetNeighbourCoords(position, direction);
				value = GetNeighbourSum(grid, position);
				grid[position] = value;
//			    PaintGrid(grid, targetNumber);
			}
		    
			return value;
		}

		private int GetNeighbourSum(Dictionary<Tuple<int,int>,int> grid, Tuple<int,int> position)
		{
			var positionL = GetNeighbourCoords(position, Direction.Left);
			var positionR = GetNeighbourCoords(position, Direction.Right);

			int sum = GetNeighbourValue(grid, position, Direction.Up)
				+ GetNeighbourValue(grid, position, Direction.Down)
				+ GetNeighbourValue(grid, position, Direction.Left)
				+ GetNeighbourValue(grid, position, Direction.Right)
				+ GetNeighbourValue(grid, positionL, Direction.Up)
				+ GetNeighbourValue(grid, positionL, Direction.Down)
				+ GetNeighbourValue(grid, positionR, Direction.Up)
				+ GetNeighbourValue(grid, positionR, Direction.Down);
			return sum;
		}

		private void PaintGrid(Dictionary<Tuple<int, int>, int> grid, int maxNumber)
		{
			int minX = int.MaxValue;
			int minY = int.MaxValue;
			int maxX = int.MinValue;
			int maxY = int.MinValue;

			foreach (Tuple<int,int> position in grid.Keys)
			{
				minX = Math.Min(position.Item1, minX);
				minY = Math.Min(position.Item2, minY);
				maxX = Math.Max(position.Item1, maxX);
				maxY = Math.Max(position.Item2, maxY);
			}

			minX--;
			minY--;
			maxX++;
			maxY++;
			Console.Clear();
			Console.SetCursorPosition(0,0);
			string numberFormat = "";
			string maxAsString = maxNumber.ToString();
			for (int i = 0; i < maxAsString.Length; i++)
			{
				numberFormat += "0";
			}
			for (int y = minY; y <= maxY; y++)
			{
				for (int x = minX; x <= maxX; x++)
				{
					Tuple<int,int> position = new Tuple<int, int>(x, y);
					if (grid.ContainsKey(position))
					{
						Console.Write("{0} ", grid[position].ToString(numberFormat));
					}
					else
					{
						Console.Write(" ".PadLeft(maxAsString.Length + 1, '_'));
					}
				}
				Console.Write("\r\n");
			}
			
		}

		private int GetManhattanDistance(Tuple<int, int> a, Tuple<int, int> b)
		{
			return Math.Abs(b.Item1 - a.Item1) + Math.Abs(b.Item2 - a.Item2);
		}
        
	    protected override object GetSolutionPart1()
	    {
            /*
             * --- Day 3: Spiral Memory ---

                You come across an experimental new kind of memory stored on an infinite two-dimensional grid.

                Each square on the grid is allocated in a spiral pattern starting at a location marked 1 and then counting up while spiraling outward. For example, the first few
                squares are allocated like this:

                17  16  15  14  13
                18   5   4   3  12
                19   6   1   2  11
                20   7   8   9  10
                21  22  23---> ...

                While this is very space-efficient (no squares are skipped), requested data must be carried back to square 1 (the location of the only access port for this memory system)
                by programs that can only move up, down, left, or right. They always take the shortest path: the Manhattan Distance between the location of the data and square 1.

                For example:

                    Data from square 1 is carried 0 steps, since it's at the access port.
                    Data from square 12 is carried 3 steps, such as: down, left, left.
                    Data from square 23 is carried only 2 steps: up twice.
                    Data from square 1024 must be carried 31 steps.

                How many steps are required to carry the data from the square identified in your puzzle input all the way to the access port?

             */
            #region Testrun

		    int maxSquare = 1024;
		    Tuple<int, int> x = CreateAndFillGrid(maxSquare);
		    Tuple<int,int> startPoint = new Tuple<int, int>(0, 0);
		    var distance = GetManhattanDistance(x, startPoint);
		    if (distance != 31)
		    {
			    throw new Exception("WTF");
		    }
            #endregion

		    maxSquare = Input.ToInt();
		    x = CreateAndFillGrid(maxSquare);
		    distance = GetManhattanDistance(x, startPoint);
	        return distance;
	    }

	    protected override object GetSolutionPart2()
	    {
            /*
             * 
             */
	        #region Testrun
	        
	        #endregion

		    int maxSquare = Input.ToInt();
		    var x = CreateAndFillSumGrid(maxSquare);
		    return x;
	    }
	}
}
