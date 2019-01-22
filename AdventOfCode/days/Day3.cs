using System.Collections.Generic;

namespace AdventOfCode.Days
{
    public class Day3 : Day
    {
        public Day3() : base(3) { }
        
        protected override object GetSolutionPart1()
        {
            HashSet<string> hashset = new HashSet<string>();
            int x = 0;
            int y = 0;
            hashset.Add(x + "/" + y);
            for (int i = 0; i < Input.Length; i++)
            {
                switch (Input[i])
                {
                    case '^':
                        y++;
                        break;
                    case 'v':
                        y--;
                        break;
                    case '<':
                        x--;
                        break;
                    case '>':
                        x++;
                        break;
                }
                hashset.Add(x + "/" + y);
            }

            return hashset.Count.ToString();
        }

        protected override object GetSolutionPart2()
        {
            HashSet<string> hashset = new HashSet<string>();
            int x1 = 0;
            int x2 = 0;
            int y1 = 0;
            int y2 = 0;
            hashset.Add(x1 + "/" + y1); //add starting location
            bool robo = false;
            for (int i = 0; i < Input.Length; i++)
            {
                switch (Input[i])
                {
                    case '^':
                        if (robo)
                        {
                            y2++;
                        }
                        else
                        {
                            y1++;
                        }
                        break;
                    case 'v':
                        if (robo)
                        {
                            y2--;
                        }
                        else
                        {
                            y1--;
                        }
                        break;
                    case '<':
                        if (robo)
                        {
                            x2--;
                        }
                        else
                        {
                            x1--;
                        }
                        break;
                    case '>':
                        if (robo)
                        {
                            x2++;
                        }
                        else
                        {
                            x1++;
                        }
                        break;
                }
                if (robo)
                {
                    hashset.Add(x2 + "/" + y2);
                }
                else
                {
                    hashset.Add(x1 + "/" + y1);
                }
                robo = !robo;
            }

            return hashset.Count.ToString();
        }
    }
}
