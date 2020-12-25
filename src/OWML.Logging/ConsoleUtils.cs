using System;
using OWML.Common;

namespace OWML.Logging
{
	public static class ConsoleUtils
	{
		public static void WriteByType(MessageType type, string line)
		{
			if (string.IsNullOrEmpty(line))
			{
				return;
			}

			Console.ForegroundColor = type switch
			{
				MessageType.Error => ConsoleColor.Red,
				MessageType.Warning => ConsoleColor.Yellow,
				MessageType.Success => ConsoleColor.Green,
				MessageType.Message => ConsoleColor.White,
				MessageType.Info => ConsoleColor.Cyan,
				MessageType.Fatal => ConsoleColor.Magenta,
				MessageType.Debug => ConsoleColor.DarkGray,
				_ => ConsoleColor.White
			};

			Console.WriteLine(line);
		}
	}
}
