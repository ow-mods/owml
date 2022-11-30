using System;
using OWML.Common;

namespace OWML.Logging
{
	public abstract class ModConsole : IModConsole
	{
		/*
		 * For logging to console :
		 * Unity-side - ModSocketOutput in unity-side -> SocketListener in owml-side -> Console
		 * Owml-side - OutputWriter in owml-side -> Console
		 * 
		 * For logging to manager :
		 * Unity-side - ModSocketOutput in unity-side -> Manager
		 * Owml-side - ModSocketOutput in owml-side -> Manager
		 */

		[Obsolete("Use ModHelper.Console instead.")]
		public static ModConsole OwmlConsole { get; private set; }

		protected readonly IModManifest Manifest;
		protected readonly IOwmlConfig OwmlConfig;

		public abstract void WriteLine(string line);

		public abstract void WriteLine(string line, MessageType type);

		public abstract void WriteLine(string line, MessageType type, string senderType);

		protected ModConsole(IOwmlConfig config, IModManifest manifest)
		{
			Manifest = manifest;
			OwmlConfig = config;

			if (Manifest.Name == Constants.OwmlTitle)
			{
				OwmlConsole = this;
			}
		}
	}
}
