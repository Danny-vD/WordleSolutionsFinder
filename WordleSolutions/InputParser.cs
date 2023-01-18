using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WordleSolutions
{
	public static class InputParser
	{
		public static string[] HandleInput(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return SolutionsHolder.Solutions.Split('\n');
			}

			string[] data = input.Split(' ');

			string currentClue = data[0];
			string excludedChars = data.Length > 1 ? data[1] : string.Empty;
			string includedChars = data.Length > 2 ? data[2] : string.Empty;

			if (InvalidInput(currentClue))
			{
				Console.WriteLine("Invalid input, use only letters and underscores!\nType /help for more information");
				return Array.Empty<string>();
			}

			string[] specialString = GetCharsBetweenAandB(currentClue, '[', ']');
			currentClue = EnforceLength(currentClue, 5, specialString);

			foreach (string special in specialString)
			{
				foreach (char c in special.Where(c => c != '[' && c != ']' && !includedChars.Contains(c)))
				{
					includedChars += c;
				}
			}

			foreach (char letter in currentClue.Where(char.IsLetter))
			{
				if (!includedChars.Contains(letter))
				{
					includedChars += letter;
				}
			}

			return SolutionSearcher.GetPossibleSolutions(currentClue, includedChars, excludedChars, CharCount(currentClue, '['));
		}

		// invalid if the character is not a letter and none of the special allowed characters
		private static bool InvalidInput(string input)
		{
			if (input.Contains("[]"))
			{
				return true;
			}

			if (CharCount(input, '[') > 5)
			{
				return true;
			}

			if (CharCount(input, '[') != CharCount(input, ']'))
			{
				return true;
			}

			return input.ToCharArray().Any(c => !char.IsLetter(c) && c != '_' && c != '[' && c != ']');
		}

		private static string EnforceLength(string @string, int desiredLength, IReadOnlyCollection<string> countAs1Char = null, char addCharToEnd = '_')
		{
			int actualLength = @string.Length;

			if (countAs1Char != null)
			{
				// actual length = string without special strings + 1 char for each special string
				int subtractLength = countAs1Char.Sum(s => s.Length) - countAs1Char.Count;
				actualLength -= subtractLength;
			}

			if (actualLength <= desiredLength)
			{
				while (actualLength < desiredLength)
				{
					@string += addCharToEnd;
					++actualLength;
				}

				return @string;
			}

			if (countAs1Char == null || countAs1Char.Count == 0 || !countAs1Char.Any(@string.Contains))
			{
				return @string.Substring(0, desiredLength);
			}

			List<string> specialStrings = countAs1Char.ToList();
			specialStrings.RemoveAll(substring => !@string.Contains(substring));
			
			StringBuilder stringBuilder = new StringBuilder(desiredLength);
			int currentLength = 0;

			int stringLength = @string.Length;
			
			for (int i = 0; i < stringLength; i++)
			{
				char letter = @string[i];

				bool addedSpecialString = false;
					
				foreach (string specialString in specialStrings.Where(specialString => specialString[0] == letter))
				{
					int specialStringLength = specialString.Length;
					
					if (i + specialStringLength >= stringLength)
					{
						continue;
					}

					string substring = @string.Substring(i, specialStringLength);

					if (substring == specialString)
					{
						stringBuilder.Append(substring);
						addedSpecialString = true;

						i += specialStringLength - 1;
						break;
					}
				}

				if (!addedSpecialString)
				{
					stringBuilder.Append(letter);
				}

				if (++currentLength == desiredLength)
				{
					break;
				}
			}

			return stringBuilder.ToString();
		}

		private static string[] GetCharsBetweenAandB(string input, char a, char b, int startIndex = 0)
		{
			int count = CharCount(input, a);
			List<string> betweenCollection = new List<string>(2);

			int indexB = startIndex;

			for (int i = 0; i < count; i++)
			{
				int indexA = input.IndexOf(a, indexB);

				if (indexA < startIndex || indexB == input.Length - 1)
				{
					continue;
				}

				indexB = input.IndexOf(b, indexB + 1);

				string inBetween = input.Substring(indexA, indexB - indexA + 1);
				betweenCollection.Add(inBetween);
			}

			return betweenCollection.ToArray();
		}

		public static int CharCount(string input, char character)
		{
			return input.Count(c => c == character);
		}
	}
}