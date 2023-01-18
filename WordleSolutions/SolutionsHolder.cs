using System;
using System.Linq;
using System.Net;

namespace WordleSolutions
{
	public static class SolutionsHolder
	{
		private const string solutionsUri = "https://static.nytimes.com/newsgraphics/2022/01/25/wordle-solver/assets/solutions.txt";

		public static string Solutions { get; private set; }

		public static void LoadSolutions()
		{
			try
			{
				WebClient webClient = new WebClient();
				Solutions = webClient.DownloadString(solutionsUri);

				if (Solutions.Last() != '\n')
				{
					Solutions += "\n"; // Put a newline at the end so that we can search for .....\n and still detect the last one
				}
			}
			catch
			{
				Console.WriteLine("Could not load the Wordle Solutions from the webpage!\nUsing predefined list instead.\n");

				Solutions = OfflineSolutions.SOLUTIONS;
			}
		}
	}
}