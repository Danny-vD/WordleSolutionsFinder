using System;
using System.Collections.Generic;
using System.Linq;

namespace WordleSolutions
{
	internal static class Program
	{
		private const string helpMessage = "Type a 5 letter word using letters and/or underscores " +
										   "(where a underscore is a wildcard and square brackets are used to signify a letter is not in a given position)" +
										   "\nThe allowed format is \"Guess ExcludedLetters IncludedLetters\"\n\n" +
										   "example: wh[e]__ l i\n\n" +
										   "Allowed commands: /quit /exit /new /clear /help /c /count\n";

		public static void Main(string[] args)
		{
			Console.Title = "Wordle Solutions Finder";

			SolutionsHolder.LoadSolutions();
			Console.WriteLine(helpMessage);

			bool onlyShowAmount = false;

			for (;;)
			{
				string input = Console.ReadLine()?.ToLower();

				if (input == "/quit" || input == "/exit")
				{
					break;
				}

				if (input == "/new" || input == "/clear")
				{
					Console.Clear();
					continue;
				}

				if (input == "/c" || input == "/count")
				{
					onlyShowAmount ^= true;

					if (onlyShowAmount)
					{
						Console.WriteLine("Will now only show the amount of solutions left\n");
					}
					else
					{
						Console.WriteLine("Will now show all the solutions left\n");
					}

					continue;
				}

				if (input == "/help" || input == string.Empty)
				{
					Console.WriteLine(helpMessage);
					continue;
				}

				Console.Write(Environment.NewLine);

				string[] possibleSolutions = InputParser.HandleInput(input);
				List<string> solutionsList = possibleSolutions.ToList();
				solutionsList.RemoveAll(item => item.Equals(string.Empty));

				if (onlyShowAmount)
				{
					int length = solutionsList.Count;

					string messsage;

					if (length == 0)
					{
						messsage = "There are no solutions left with these clues!";
					}
					else
					{
						messsage = length > 1 ? $"There are {length} solutions left" : $"There is only {length} solution left!";
					}

					Console.WriteLine(messsage);
				}
				else
				{
					foreach (string possibleSolution in solutionsList)
					{
						Console.WriteLine(possibleSolution);
					}
				}

				Console.WriteLine(Environment.NewLine);
			}
		}
	}
}