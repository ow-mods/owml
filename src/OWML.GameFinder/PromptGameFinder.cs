using System;
using OWML.Common;

namespace OWML.GameFinder
{
	public class PromptGameFinder : BaseFinder
	{
		public PromptGameFinder(IOwmlConfig config, IModConsole writer)
			: base(config, writer)
		{
		}

		public override string FindGamePath()
		{
			var gamePath = Config.GamePath;
			while (!IsValidGamePath(gamePath))
			{
				Writer.WriteLine($"Game not found at {gamePath}");
				Writer.WriteLine("Please enter the correct game path:");
				gamePath = Console.ReadLine()?.Trim();
			}
			return gamePath;
		}
	}
}