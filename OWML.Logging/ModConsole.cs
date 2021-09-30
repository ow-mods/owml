using OWML.Common;
using System;

namespace OWML.Logging
{
	public abstract class ModConsole : IModConsole
	{
		[Obsolete("Use ModHelper.Console instead.")]
		public static IModConsole OwmlConsole { get; private set; }

		protected readonly IModLogger Logger;
		protected readonly IModManifest Manifest;
		protected readonly IOwmlConfig OwmlConfig;

		public abstract void WriteLine(string line);

		public abstract void WriteLine(string line, MessageType type);

		public abstract void WriteLine(string line, MessageType type, string senderType);

		protected ModConsole(IOwmlConfig config, IModLogger logger, IModManifest manifest)
		{
			Logger = logger;
			Manifest = manifest;
			OwmlConfig = config;

			if (Manifest.Name == Constants.OwmlTitle)
			{
				OwmlConsole = this;
			}
		}
	}
}
