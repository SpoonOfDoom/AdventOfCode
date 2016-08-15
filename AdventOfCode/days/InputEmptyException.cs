using System;

namespace AdventOfCode.Days
{
    /// <summary>
    /// Exception for the case of an empty input string.
    /// </summary>
    public class InputEmptyException : Exception
    {
        private const string defaultMessage = "The input string is null or empty.";
        public  InputEmptyException() : base(defaultMessage) { }
    }
}