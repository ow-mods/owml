using System;
using System.Runtime.InteropServices;
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

			if (!isValidFormat)
			{
				_writer.WriteLine("Invalid game version format. You can disable this check in the OWML config.", MessageType.Warning);
			}
			else if (gameVersion > maxVersion)
			{
				_writer.WriteLine($"New Outer Wilds version ({gameVersionString}). This version of OWML was built for {maxVersion}. You can disable this check in the OWML config.", MessageType.Warning);
			}
			else if (gameVersion < minVersion)
			{
				_writer.WriteLine($"Outdated Outer Wilds version ({gameVersionString}). The oldest compatible version is {minVersion}. You can disable this check in the OWML config.", MessageType.Fatal);
			}
		}
	}
}
