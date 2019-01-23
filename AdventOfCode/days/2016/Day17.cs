﻿using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Tools;

namespace AdventOfCode.Days._2016
{
    // ReSharper disable once UnusedMember.Global
    class Day17 : Day
    {
        public Day17() : base(2016, 17) {}

        private class GameState : ISearchNode
        {
            public static string PassCode;
            public static char[] OpenCharacters = new[] {'b', 'c', 'd', 'e', 'f'};
            public static Direction[] PossibleDirections = new[] {Direction.U, Direction.D, Direction.L, Direction.R};
            private string stringHash;
            private string roomHash;

            public enum Direction
            {
                U,
                D,
                L,
                R
            }
            

            public string RoomHash
            {
                get
                {
                    if (roomHash == null)
                    {
                        BuildRoomHash();
                    }
                    return roomHash;
                }
            }

            public struct RoomCoordinates
            {
                public int X;
                public int Y;

                public RoomCoordinates Move(Direction direction)
                {
                    switch (direction)
                    {
                        case Direction.U:
                            return new RoomCoordinates {X = X, Y = Y - 1};
                        case Direction.D:
                            return new RoomCoordinates {X = X, Y = Y + 1};
                        case Direction.L:
                            return new RoomCoordinates {X = X - 1, Y = Y};
                        case Direction.R:
                            return new RoomCoordinates {X = X + 1, Y = Y};
                        default:
                            throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
                    }
                }

                /// <summary>Gibt den voll qualifizierten Typnamen dieser Instanz zurück.</summary>
                /// <returns>Eine <see cref="T:System.String" />-Klasse, die den voll qualifizierten Typnamen enthält.</returns>
                public override string ToString()
                {
                    return string.Concat(X, "-", Y);
                }
            }

            public RoomCoordinates Position;
            public int Cost { get; set; }

            public List<object> Actions
            {
                get { return Steps.Select(s => s as object).ToList(); }
                set { Steps = value.Select(v => (Direction) v).ToList(); }
            }

            private List<Direction> Steps { get; set; }
            public string VerboseInfo { get; }

            public string StringHash
            {
                get
                {
                    if (stringHash == null)
                    {
                        CreateHash();
                    }
                    return stringHash;
                }
            }

            public long NumericHash { get; }

            
            public HashSet<ExpandAction> ExpandNode()
            {
                HashSet<ExpandAction> actions = new HashSet<ExpandAction>();

                for (int i = 0; i < PossibleDirections.Length; i++)
                {
                    if (Position.X == 0 && PossibleDirections[i] == Direction.L)
                    {
                        continue;
                    }
                    else if (Position.X == 3 && PossibleDirections[i] == Direction.R)
                    {
                        continue;
                    }
                    else if (Position.Y == 0 && PossibleDirections[i] == Direction.U)
                    {
                        continue;
                    }
                    else if (Position.Y == 3 && PossibleDirections[i] == Direction.D)
                    {
                        continue;
                    }
                    
                    if (OpenCharacters.Contains(RoomHash[i]))
                    {
                        GameState newState = CloneAndMove(PossibleDirections[i]);
                        if (newState.Position.X < 0 || newState.Position.X > 3 || newState.Position.Y < 0 || newState.Position.Y > 3)
                        {
                            continue;
                        }
                        actions.Add(new ExpandAction
                        {
                            Action = PossibleDirections[i],
                            Cost = 1,
                            Result = newState
                        });
                    }
                }
                
                return actions;
            }

            public bool Equals(ISearchNode otherState)
            {
                return StringHash == otherState.StringHash;
            }

            public bool IsGoalState(ISearchNode goalState = null)
            {
                return Position.Equals(((GameState) goalState).Position);
            }

            public float GetHeuristic(ISearchNode goalState = null)
            {
                int manhattanDistance = ((GameState) goalState).Position.Y - Position.Y + ((GameState) goalState).Position.X - Position.X;
                //todo: create heuristic closer to reality. Consider number of open doors?
                return manhattanDistance;
            }

            public void CreateHash()
            {
                stringHash = string.Concat(Position.ToString(), "$", RoomHash);
            }

            public GameState CloneAndMove(Direction direction)
            {
                var newState = new GameState
                {
                    Steps = new List<Direction>(Steps),
                    Cost = Cost,
                    Position = Position.Move(direction)
                };
                newState.Steps.Add(direction);
                return newState;
            }

            private void BuildRoomHash()
            {
                roomHash = Toolbox.GetHashString(PassCode + string.Join("", Steps), new MD5Digest());
            }
        }

        protected override object GetSolutionPart1()
        {
            /*
             * --- Day 17: Two Steps Forward ---

                    You're trying to access a secure vault protected by a 4x4 grid of small rooms connected by doors. You start in the top-left room (marked S), and you can access the vault
                    (marked V) once you reach the bottom-right room:

                    #########
                    #S| | | #
                    #-#-#-#-#
                    # | | | #
                    #-#-#-#-#
                    # | | | #
                    #-#-#-#-#
                    # | | |  
                    ####### V

                    Fixed walls are marked with #, and doors are marked with - or |.

                    The doors in your current room are either open or closed (and locked) based on the hexadecimal MD5 hash of a passcode (your puzzle input) followed by a sequence of
                    uppercase characters representing the path you have taken so far (U for up, D for down, L for left, and R for right).

                    Only the first four characters of the hash are used; they represent, respectively, the doors up, down, left, and right from your current position. Any b, c, d, e, or f
                    means that the corresponding door is open; any other character (any number or a) means that the corresponding door is closed and locked.

                    To access the vault, all you need to do is reach the bottom-right room; reaching this room opens the vault and all doors in the maze.

                    For example, suppose the passcode is hijkl. Initially, you have taken no steps, and so your path is empty: you simply find the MD5 hash of hijkl alone.
                    The first four characters of this hash are ced9, which indicate that up is open (c), down is open (e), left is open (d), and right is closed and locked (9).
                    Because you start in the top-left corner, there are no "up" or "left" doors to be open, so your only choice is down.

                    Next, having gone only one step (down, or D), you find the hash of hijklD. This produces f2bc, which indicates that you can go back up, left (but that's a wall), or right.
                    Going right means hashing hijklDR to get 5745 - all doors closed and locked. However, going up instead is worthwhile: even though it returns you to the room you started in,
                    your path would then be DU, opening a different set of doors.

                    After going DU (and then hashing hijklDU to get 528e), only the right door is open; after going DUR, all doors lock. (Fortunately, your actual passcode is not hijkl).

                    Passcodes actually used by Easter Bunny Vault Security do allow access to the vault if you know the right path. For example:

                        If your passcode were ihgpwlah, the shortest path would be DDRRRD.
                        With kglvqrro, the shortest path would be DDUDRLRRUDRD.
                        With ulqzkmiv, the shortest would be DRURDRUDDLLDLUURRDULRLDUUDDDRR.

                    Given your vault's passcode, what is the shortest path (the actual path, not just the length) to reach the vault?
                    */

            AStar aStar = new AStar(AStar.NodeHashMode.String);

            GameState startState = new GameState
            {
                Position = new GameState.RoomCoordinates { X = 0, Y = 0 },
                Actions = new List<object>()
            };
            GameState goalState = new GameState
            {
                Position = new GameState.RoomCoordinates { X = 3, Y = 3 },
                Actions = new List<object>()
            };
            Tuple<List<object>, int> path;
            List<string> actions;
            string result;

            #region TestRuns
            GameState.PassCode = "ihgpwlah";
            startState = new GameState
            {
                Position = new GameState.RoomCoordinates { X = 0, Y = 0 },
                Actions = new List<object>()
            };
            goalState = new GameState
            {
                Position = new GameState.RoomCoordinates { X = 3, Y = 3 },
                Actions = new List<object>()
            };
            path = aStar.GetOptimalPath(startState, goalState);
            actions = path.Item1.Select(x => ((GameState.Direction)x).ToString()).ToList();
            result = string.Join("", actions);
            if (result != "DDRRRD")
            {
                throw new Exception("Test 1 failed. Expected: DDRRRD, actual: " + result);
            }

            GameState.PassCode = "kglvqrro";
            startState = new GameState
            {
                Position = new GameState.RoomCoordinates { X = 0, Y = 0 },
                Actions = new List<object>()
            };
            goalState = new GameState
            {
                Position = new GameState.RoomCoordinates { X = 3, Y = 3 },
                Actions = new List<object>()
            };
            path = aStar.GetOptimalPath(startState, goalState);
            actions = path.Item1.Select(x => ((GameState.Direction)x).ToString()).ToList();
            result = string.Join("", actions);
            if (result != "DDUDRLRRUDRD")
            {
                throw new Exception("Test 2 failed. Expected: DDUDRLRRUDRD, actual: " + result);
            }

            GameState.PassCode = "ulqzkmiv";
            startState = new GameState
            {
                Position = new GameState.RoomCoordinates { X = 0, Y = 0 },
                Actions = new List<object>()
            };
            goalState = new GameState
            {
                Position = new GameState.RoomCoordinates { X = 3, Y = 3 },
                Actions = new List<object>()
            };
            path = aStar.GetOptimalPath(startState, goalState);
            actions = path.Item1.Select(x => ((GameState.Direction)x).ToString()).ToList();
            result = string.Join("", actions);
            if (result != "DRURDRUDDLLDLUURRDULRLDUUDDDRR")
            {
                throw new Exception("Test 3 failed. Expected: DRURDRUDDLLDLUURRDULRLDUUDDDRR, actual: " + result);
            }

            #endregion


            GameState.PassCode = Input;
            startState = new GameState
            {
                Position = new GameState.RoomCoordinates { X = 0, Y = 0 },
                Actions = new List<object>()
            };
            goalState = new GameState
            {
                Position = new GameState.RoomCoordinates { X = 3, Y = 3 },
                Actions = new List<object>()
            };
            path = aStar.GetOptimalPath(startState, goalState);
            actions = path.Item1.Select(x => ((GameState.Direction) x).ToString()).ToList();
            result = string.Join("", actions);
            return result; //RDULRDDRRD
        }

        protected override object GetSolutionPart2()
        {
            /*
             * You're curious how robust this security solution really is, and so you decide to find longer and longer paths which still provide access to the vault.
             * You remember that paths always end the first time they reach the bottom-right room (that is, they can never pass through it, only end in it).

                For example:

                    If your passcode were ihgpwlah, the longest path would take 370 steps.
                    With kglvqrro, the longest path would be 492 steps long.
                    With ulqzkmiv, the longest path would be 830 steps long.

                What is the length of the longest path that reaches the vault?
             */

            //todo: improve execution time
            AStar aStar = new AStar(AStar.NodeHashMode.String);

            GameState startState = new GameState
            {
                Position = new GameState.RoomCoordinates { X = 0, Y = 0 },
                Actions = new List<object>()
            };
            GameState goalState = new GameState
            {
                Position = new GameState.RoomCoordinates { X = 3, Y = 3 },
                Actions = new List<object>()
            };
            Tuple<List<object>, int> path;
            int result;

            #region TestRuns
            GameState.PassCode = "ihgpwlah";
            startState = new GameState
            {
                Position = new GameState.RoomCoordinates { X = 0, Y = 0 },
                Actions = new List<object>()
            };
            goalState = new GameState
            {
                Position = new GameState.RoomCoordinates { X = 3, Y = 3 },
                Actions = new List<object>()
            };
            path = aStar.GetLongestPath(startState, goalState);
            result = path.Item2;
            if (result != 370)
            {
                throw new Exception("Test 1 failed. Expected: 370, actual: " + result);
            }

            GameState.PassCode = "kglvqrro";
            startState = new GameState
            {
                Position = new GameState.RoomCoordinates { X = 0, Y = 0 },
                Actions = new List<object>()
            };
            goalState = new GameState
            {
                Position = new GameState.RoomCoordinates { X = 3, Y = 3 },
                Actions = new List<object>()
            };
            path = aStar.GetLongestPath(startState, goalState);
            result = path.Item2;
            if (result != 492)
            {
                throw new Exception("Test 2 failed. Expected: 492, actual: " + result);
            }

            GameState.PassCode = "ulqzkmiv";
            startState = new GameState
            {
                Position = new GameState.RoomCoordinates { X = 0, Y = 0 },
                Actions = new List<object>()
            };
            goalState = new GameState
            {
                Position = new GameState.RoomCoordinates { X = 3, Y = 3 },
                Actions = new List<object>()
            };
            path = aStar.GetLongestPath(startState, goalState);
            result = path.Item2;
            if (result != 830)
            {
                throw new Exception("Test 3 failed. Expected: 830, actual: " + result);
            }

            #endregion


            GameState.PassCode = Input;
            startState = new GameState
            {
                Position = new GameState.RoomCoordinates { X = 0, Y = 0 },
                Actions = new List<object>()
            };
            goalState = new GameState
            {
                Position = new GameState.RoomCoordinates { X = 3, Y = 3 },
                Actions = new List<object>()
            };
            path = aStar.GetLongestPath(startState, goalState);
            result = path.Item2;
            return result; //752
        }
    }
}
