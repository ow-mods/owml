namespace OWML.Common
{
	public interface IModConsole
	{
		void WriteLine(string line);

		void WriteLine(string line, MessageType type);

		void WriteLine(string line, MessageType type, string senderType);
	}
}
