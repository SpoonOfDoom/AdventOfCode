using System;
using AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day25 : Day
    {
        public Day25() : base(25) { }
        int targetRow, targetColumn;
        long startCode = 20151125;

        const long multiplier = 252533;
        const long divider = 33554393;

        private long GenerateNextCode(long code)
        {
            if (code == 0)
            {
                return startCode;
            }
            return (code * multiplier) % divider;
        }

        private long GetItemsForRow(long row)
        {
            return ((row*row) + row) / 2;
        }

        private long GetCodeFor(long row, long column)
        {
            long rowNeeded = column + (row - 1);
            long itemCountTemp = GetItemsForRow(rowNeeded - 1);
            long indexOfCode = itemCountTemp + column;
            long code = 0;

            for (int i = 0; i < indexOfCode; i++)
            {
                code = GenerateNextCode(code);
            }

            return code;
        }

        public override string GetSolutionPart1()
        {
            targetRow = input.Split(',')[0].ToInt();
            targetColumn = input.Split(',')[1].ToInt();
            
            long code = GetCodeFor(targetRow, targetColumn);
            
            return code.ToString(); //19980801
        }
        
        public override string GetSolutionPart2()
        {
            return "Hooray!";
        }
    }
}