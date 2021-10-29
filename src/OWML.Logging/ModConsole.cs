using OWML.Common;

namespace OWML.Logging
{
	public abstract class ModConsole : IModConsole
	{
		protected readonly IModManifest Manifest;
		protected readonly IOwmlConfig OwmlConfig;

		public abstract void WriteLine(string line);

		public abstract void WriteLine(string line, MessageType type);

		public abstract void WriteLine(string line, MessageType type, string senderType);

		protected ModConsole(IOwmlConfig config, IModManifest manifest)
		{
			Manifest = manifest;
			OwmlConfig = config;
		}
	}
}
