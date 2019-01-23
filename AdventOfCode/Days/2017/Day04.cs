using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Days._2017
{
    // ReSharper disable once UnusedMember.Global
    class Day04 : Day
	{
		public Day04() : base(2017, 4) {}

	    private bool IsPassPhraseValid(string phrase, bool part2Mode = false)
	    {
	        string[] words = phrase.Split(' ');

	        if (part2Mode)
	        {
	            var orderedWords = words.Select(s => string.Join("", s.OrderBy(c => c))).ToList();
	            return orderedWords.Count == orderedWords.Distinct().Count();
            }

	        return words.Length == words.Distinct().Count();
	    }

	    private int GetValidPhraseCount(List<string> phrases, bool part2Mode = false)
	    {
	        return phrases.Count(p => IsPassPhraseValid(p, part2Mode));
	    }

	    protected override object GetSolutionPart1()
	    {
            /*
             * --- Day 4: High-Entropy Passphrases ---

                A new system policy has been put in place that requires all accounts to use a passphrase instead of simply a password. A passphrase consists of a series of words
                (lowercase letters) separated by spaces.

                To ensure security, a valid passphrase must contain no duplicate words.

                For example:

                    aa bb cc dd ee is valid.
                    aa bb cc dd aa is not valid - the word aa appears more than once.
                    aa bb cc dd aaa is valid - aa and aaa count as different words.

                The system's full passphrase list is available as your puzzle input. How many passphrases are valid?


             */
            #region Testrun
            var testInputs = new Dictionary<string, bool>
                                                 {
                                                    {"aa bb cc ee", true },
                                                    {"aa bb cc dd aa", false },
                                                    {"aa bb cc dd aaa", true },
                                                 };
	        foreach (KeyValuePair<string, bool> pair in testInputs)
	        {
	            bool testResult = IsPassPhraseValid(pair.Key);
	            if (testResult != pair.Value)
	            {
	                throw new Exception($"Test failed for {pair.Key}! Expected: {pair.Value}, Actual: {testResult}");
	            }
	        }
            #endregion

	        int result = GetValidPhraseCount(InputLines);
	        return result;
	    }

	    protected override object GetSolutionPart2()
	    {
            /*
             * --- Part Two ---

                For added security, yet another system policy has been put in place. Now, a valid passphrase must contain no two words that are anagrams of each other -
                that is, a passphrase is invalid if any word's letters can be rearranged to form any other word in the passphrase.

                For example:

                    abcde fghij is a valid passphrase.
                    abcde xyz ecdab is not valid - the letters from the third word can be rearranged to form the first word.
                    a ab abc abd abf abj is a valid passphrase, because all letters need to be used when forming another word.
                    iiii oiii ooii oooi oooo is valid.
                    oiii ioii iioi iiio is not valid - any of these words can be rearranged to form any other word.

                Under this new system policy, how many passphrases are valid?

             */
            #region Testrun
	        var testInputs = new Dictionary<string, bool>
	                         {
	                             {"abcde fghij", true },
	                             {"abcde xyz ecdab", false },
	                             {"a ab abc abd abf abj", true },
	                             {"iiii oiii ooii oooi oooo", true },
	                             {"oiii ioii iioi iiio", false },
	                         };
	        foreach (KeyValuePair<string, bool> pair in testInputs)
	        {
	            bool testResult = IsPassPhraseValid(pair.Key, part2Mode:true);
	            if (testResult != pair.Value)
	            {
	                throw new Exception($"Test failed for {pair.Key}! Expected: {pair.Value}, Actual: {testResult}");
	            }
	        }
            #endregion

            int result = GetValidPhraseCount(InputLines, part2Mode:true);
            return result;
	    }
	}
}
