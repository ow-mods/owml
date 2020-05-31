using System;
using System.IO;
using Microsoft.Win32;
using OWML.Common;

namespace OWML.GameFinder
{
    public class SteamGameFinder : BaseFinder
    {
        private const string RegistryPath = @"SOFTWARE\Valve\Steam";
        private const string RegistryName = "SteamPath";
        private const string GameLocation = "steamapps/common/Outer Wilds";
        private const string LibraryFoldersPath = "steamapps/libraryfolders.vdf";
        private const string LibrarySearchPattern = "\"{n}\"\t\t\"";
        
        public SteamGameFinder(IOwmlConfig config, IModConsole writer) : base(config, writer)
        {
        }

        public override string FindGamePath()
        {
            var key = Registry.CurrentUser.OpenSubKey(RegistryPath);
            var steamPath = (string)key?.GetValue(RegistryName);
            if (string.IsNullOrEmpty(steamPath))
            {
                Writer.WriteLine("Steam not found in Registry.");
                return null;
            }
            var defaultLocation = $"{steamPath}/{GameLocation}";
            if (IsValidGamePath(defaultLocation))
            {
                return defaultLocation;
            }
            var libraryFoldersFile = $"{steamPath}/{LibraryFoldersPath}";
            if (!File.Exists(libraryFoldersFile))
            {
                Writer.WriteLine($"Steam library folders file not found: {libraryFoldersFile}");
                return null;
            }
            var libraryFoldersContent = File.ReadAllText(libraryFoldersFile);
            for (var i = 1; i < 10; i++)
            {
                var libraryStart = LibrarySearchPattern.Replace("{n}", i.ToString());
                var startIndex = libraryFoldersContent.IndexOf(libraryStart, StringComparison.Ordinal);
                if (startIndex < 0)
                {
                    Writer.WriteLine("Game not found in custom Steam library.");
                    return null;
                }
                var afterStartIndex = startIndex + libraryStart.Length;
                var endIndex = libraryFoldersContent.IndexOf('\"', afterStartIndex);
                var length = endIndex - afterStartIndex;
                var libraryPath = libraryFoldersContent.Substring(afterStartIndex, length);
                var gamePath = $"{libraryPath}/{GameLocation}";
                if (IsValidGamePath(gamePath))
                {
                    return gamePath;
                }
            }
            Writer.WriteLine("Game not found in Steam.");
            return null;
        }

    }
}
