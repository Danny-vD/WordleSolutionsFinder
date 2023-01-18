using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WordleSolutions
{
	public static class SolutionSearcher
	{
		public static string[] GetPossibleSolutions(string currentClue, string includedChars, string excludedChars, int specialCount = 0)
		{
			// replace the underscores with the regex any symbol
			currentClue = currentClue.Replace('_', '.');

			string modifiedClue = currentClue;

			int index = 0;

			// Turn any [{chars}] into [^{chars}] (the regex for not this character)
			for (int i = 0; i < specialCount; i++)
			{
				index        = modifiedClue.IndexOf('[', index) + 1;
				modifiedClue = modifiedClue.Insert(index, "^");
			}

			modifiedClue += '\n';

			MatchCollection matchCollection = Regex.Matches(SolutionsHolder.Solutions, modifiedClue);
			string solutions = ToString(matchCollection);

			if (excludedChars != string.Empty)
			{
				solutions = FilterByExcluded(solutions, excludedChars, includedChars);
			}

			if (includedChars != string.Empty)
			{
				solutions = FilterByIncluded(solutions, includedChars);
			}

			return solutions.Split('\n');
		}

		private static string FilterByIncluded(string solutions, string included)
		{
			char[] includedChars = included.ToCharArray();

			string filteredSolutions = string.Empty;

			foreach (string solution in solutions.Split('\n'))
			{
				for (int i = 0; i < includedChars.Length; i++)
				{
					// If the charCount of a letter is less than we want, don't include as possible solution
					if (InputParser.CharCount(solution, includedChars[i]) < InputParser.CharCount(included, includedChars[i]))
					{
						break;
					}

					if (i == includedChars.Length - 1) // last iteration
					{
						filteredSolutions += solution.Trim() + Environment.NewLine;
					}
				}
			}

			return filteredSolutions;
		}

		private static string FilterByExcluded(string solutions, string excludedChars, string includedChars)
		{
			List<char> limitToOne = new List<char>(2);

			// Make sure we don't exclude any letter listed as included
			foreach (char letter in includedChars)
			{
				if (excludedChars.Contains(letter))
				{
					excludedChars = excludedChars.Replace(letter.ToString(), string.Empty);
					limitToOne.Add(letter);
				}
			}

			if (limitToOne.Count > 0)
			{
				// If a letter is both in the included and excluded list, allow it only once in the word (e.g. no double 'L' in 'allow')
				solutions = FilterToSingleLetter(solutions, limitToOne.ToArray());
			}

			if (excludedChars == string.Empty)
			{
				return solutions;
			}

			// Make sure the matching solutions don't contain any whitespaces (which would lead to half words in some cases)
			string pattern = $"[^{excludedChars + Environment.NewLine}]";

			string copyPattern = pattern;

			for (int i = 0; i < 4; i++)
			{
				pattern += copyPattern;
			}

			pattern += Environment.NewLine;

			MatchCollection matchCollection = Regex.Matches(solutions, pattern);
			return ToString(matchCollection);
		}

		private static string FilterToSingleLetter(string solutions, char[] mayOnlyContainOne)
		{
			string filteredSolutions = string.Empty;

			foreach (string solution in solutions.Split('\n'))
			{
				for (int i = 0; i < mayOnlyContainOne.Length; i++)
				{
					char letter = mayOnlyContainOne[i];

					if (InputParser.CharCount(solution, letter) > 1)
					{
						break;
					}

					if (i == mayOnlyContainOne.Length - 1) // last iteration
					{
						filteredSolutions += solution.Trim() + Environment.NewLine;
					}
				}
			}

			return filteredSolutions;
		}

		private static string ToString(MatchCollection matchCollection)
		{
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < matchCollection.Count; i++)
			{
				builder.AppendLine(matchCollection[i].Value.Trim());
			}

			return builder.ToString();
		}
	}
}