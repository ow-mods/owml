using System;
using System.Collections.Generic;
using OWML.Common;

namespace OWML.Logging
{
	public static class ConsoleUtils
	{
		private static readonly Dictionary<MessageType, ConsoleColor> MessageTypeColors = new()
		{
			{ MessageType.Error, ConsoleColor.Red },
			{ MessageType.Warning, ConsoleColor.Yellow },
			{ MessageType.Success, ConsoleColor.Green },
			{ MessageType.Message, ConsoleColor.Gray },
			{ MessageType.Info, ConsoleColor.Cyan },
			{ MessageType.Fatal, ConsoleColor.Magenta }
		};

		public static void WriteByType(MessageType type, string line)
		{
			if (string.IsNullOrEmpty(line))
			{
				return;
			}

			Console.ForegroundColor = MessageTypeColors.ContainsKey(type)
				? MessageTypeColors[type]
				: ConsoleColor.Gray;

			Console.WriteLine(line);
			Console.ForegroundColor = ConsoleColor.Gray;
		}
	}
}
