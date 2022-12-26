using System;
using System.Windows.Forms;
using OWML.Common;

namespace OWML.Launcher
{
	public class GameVersionHandler : IGameVersionHandler
	{
		private readonly IGameVersionReader _versionReader;
		private readonly IModConsole _writer;
		private readonly IModManifest _owmlManifest;
		private readonly IProcessHelper _processHelper;

		public GameVersionHandler(IGameVersionReader versionReader, IModConsole writer, IModManifest owmlManifest, IProcessHelper processHelper)
		{
			_versionReader = versionReader;
			_writer = writer;
			_owmlManifest = owmlManifest;
			_processHelper = processHelper;
		}

		public void CompareVersions()
		{
			var gameVersionString = _versionReader.GetGameVersion();

			_writer.WriteLine($"Game version: {gameVersionString}", MessageType.Info);
			var isValidFormat = Version.TryParse(gameVersionString, out var gameVersion);
			var minVersion = new Version(_owmlManifest.MinGameVersion);
			var maxVersion = new Version(_owmlManifest.MaxGameVersion);

			var showPopup = false;
			var reason = "";

			if (!isValidFormat)
			{
				showPopup = true;
				reason = "Invalid game version format.";
				_writer.WriteLine("Warning - Invalid game version format", MessageType.Warning);
			}
			else if (gameVersion > maxVersion)
			{
				showPopup = true;
				reason = $"New Outer Wilds version. This version of OWML was built for {maxVersion}.";
				_writer.WriteLine($"New Outer Wilds version. This version of OWML was built for {maxVersion}.", MessageType.Warning);
			}
			else if (gameVersion < minVersion)
			{
				showPopup = true;
				reason = $"Outdated Outer Wilds version. The oldest compatible version is {minVersion}.";
				_writer.WriteLine($"Outdated Outer Wilds version. The oldest compatible version is {minVersion}.", MessageType.Warning);
			}

			if (showPopup)
			{
				var result = MessageBox.Show($"OWML could not verify if it will work on this version of Outer Wilds ({gameVersionString}).\n\n" +
					$"Reason: {reason}\n\n" +
					$"Do you want to continue?", "OWML.Launcher.GameVersionHandler", MessageBoxButtons.YesNo);
				if (result == DialogResult.No)
				{
					_processHelper.KillCurrentProcess();
				}
			}
		}
	}
}
