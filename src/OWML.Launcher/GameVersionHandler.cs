using System;
using OWML.Common;

namespace OWML.Launcher
{
	public class GameVersionHandler : IGameVersionHandler
	{
		private readonly IGameVersionReader _versionReader;
		private readonly IModConsole _writer;
		private readonly IModManifest _owmlManifest;

		public GameVersionHandler(IGameVersionReader versionReader, IModConsole writer, IModManifest owmlManifest)
		{
			_versionReader = versionReader;
			_writer = writer;
			_owmlManifest = owmlManifest;
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
				_writer.WriteLine("Warning - invalid game version format", MessageType.Warning);
			}
			else if (gameVersion > maxVersion)
			{
				_writer.WriteLine("Potentially unsupported game version found, continue at your own risk", MessageType.Warning);
			}
			else if (gameVersion < minVersion)
			{
				_writer.WriteLine("Unsupported game version found, custom menus won't work.", MessageType.Warning);
			}
			else
			{
				_writer.WriteLine("Game version is supported", MessageType.Success);
			}
		}
	}
}
