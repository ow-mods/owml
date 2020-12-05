using System;
using OWML.Common.Enums;
using OWML.Common.Interfaces;
using OWML.Patcher;

namespace OWML.Launcher
{
    public class GameVersionHandler : IGameVersionHandler
    {
        private readonly GameVersionReader _versionReader;
        private readonly IModConsole _writer;
        private readonly IModManifest _owmlManifest;

        public GameVersionHandler(GameVersionReader versionReader, IModConsole writer, IModManifest owmlManifest)
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
                _writer.WriteLine("Warning - non-standard game version formatting found", MessageType.Warning);
                return;
            }

            if (gameVersion > maxVersion)
            {
                _writer.WriteLine("Potentially unsupported game version found, continue at your own risk", MessageType.Warning);
                return;
            }

            if (gameVersion < minVersion)
            {
                AnyKeyExitConsole();
                return;
            }

            _writer.WriteLine("Version is supported", MessageType.Success);
        }

        private void AnyKeyExitConsole()
        {
            _writer.WriteLine("Unsupported game version found", MessageType.Error);
            _writer.WriteLine("Press any key to exit...", MessageType.Info);
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
