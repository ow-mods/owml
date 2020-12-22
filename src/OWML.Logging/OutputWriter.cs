using System;
using System.Linq;
using OWML.Common;

namespace OWML.Logging
{
	public class OutputWriter : IModConsole
	{
		private readonly IOwmlConfig _config;

		public OutputWriter(IOwmlConfig config) => 
			_config = config;

		[Obsolete("Use WriteLine(string) or WriteLine(string, MessageType) instead.")]
		public void WriteLine(params object[] objects) =>
			WriteLine(string.Join(" ", objects.Select(o => o.ToString()).ToArray()));

		public void WriteLine(string line) =>
			WriteLine(line, MessageType.Message);

		public void WriteLine(string line, MessageType type, string senderType) =>
			WriteLine($"{senderType}: {line}", type);

		public void WriteLine(string line, MessageType type)
		{
			if (type != MessageType.Debug || _config.DebugMode)
			{
				ConsoleUtils.WriteByType(type, line);
			}
		}
	}
}
