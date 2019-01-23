﻿using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Extensions;

namespace AdventOfCode.Days._2016
{
    // ReSharper disable once UnusedMember.Global
    class Day1 : Day
	{
		public Day1() : base(2016, 1) {}
		private List<MoveCommand> commandList = new List<MoveCommand>();
		private HashSet<string> visitedLocations = new HashSet<string>();
		
		private struct MoveCommand
		{
			public char Rotation;
			public int Blocks;
		}

		private void ParseInput()
		{
			IEnumerable<string> commands = Input.Split(',').Select(s => s.Trim());
			foreach (string s in commands)
			{
				var command = new MoveCommand()
				{
					Rotation = s[0],
					Blocks = s.Substring(1).ToInt()
				};
				commandList.Add(command);
			}
		}

		private char ApplyRotation(char start, char rotation)
		{
			char newDirection;
			switch (start)
			{
				case 'n':
					newDirection = rotation == 'R' ? 'e' : 'w';
					break;
				case 'e':
					newDirection = rotation == 'R' ? 's' : 'n';
					break;
				case 's':
					newDirection = rotation == 'R' ? 'w' : 'e';
					break;
				case 'w':
					newDirection = rotation == 'R' ? 'n' : 's';
					break;
                default:
                    throw new Exception("Invalid direction");
			}
			return newDirection;
		}

		private void Move(ref int x, ref int y, int amount, char direction, List<string> locations = null )
		{
			switch (direction)
			{
				case 'n':
					if (locations != null)
					{
						for (int i = 1; i <= amount; i++)
						{
							locations.Add($"{x}|{y + i}");
						}
					}
					y += amount;
					break;
				case 'e':
					if (locations != null)
					{
						for (int i = 1; i <= amount; i++)
						{
							locations.Add($"{x+i}|{y}");
						}
					}
					x += amount;
					break;
				case 's':
					if (locations != null)
					{
						for (int i = 1; i <= amount; i++)
						{
							locations.Add($"{x}|{y - i}");
						}
					}
					y -= amount;
					break;
				case 'w':
					if (locations != null)
					{
						for (int i = 1; i <= amount; i++)
						{
							locations.Add($"{x - i}|{y}");
						}
					}
					x -= amount;
					break;
				default:
					return;
			}
		}

	    protected override object GetSolutionPart1()
		{
			ParseInput();

			int x = 0;
			int y = 0;

			char facing = 'n';

			foreach (MoveCommand command in commandList)
			{
				facing = ApplyRotation(facing, command.Rotation);
				Move(ref x, ref y, command.Blocks, facing);
			}
			int distance = x + y;
			return distance;
		}

	    protected override object GetSolutionPart2()
		{
			int x = 0;
			int y = 0;

			char facing = 'n';
			visitedLocations.Add("0-0");
			foreach (MoveCommand command in commandList)
			{
				var locations = new List<string>();
				facing = ApplyRotation(facing, command.Rotation);
				Move(ref x, ref y, command.Blocks, facing, locations);

				foreach (string s in locations)
				{
					if (visitedLocations.Contains(s))
					{
						IEnumerable<int> numbers = s.Split('|').Select(c => int.Parse(c.ToString()));
						int solution = numbers.Sum(Math.Abs);
						return solution;
					}
					visitedLocations.Add(s);
				}
				
			}
			return "-1";
		}
	}
}
