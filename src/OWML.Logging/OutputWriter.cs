using OWML.Common;

namespace OWML.Logging
{
	public class OutputWriter : IModConsole
	{
		private readonly IOwmlConfig _config;

		public OutputWriter(IOwmlConfig config) => 
			_config = config;

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
