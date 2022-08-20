using OWML.Common;

namespace OWML.ModLoader
{
	class ModVersionChecker : IModVersionChecker
	{
		private readonly IModConsole _console;
		private readonly IModManifest _owmlManifest;

		public ModVersionChecker(IModConsole console, IModManifest owmlManifest)
		{
			_console = console;
			_owmlManifest = owmlManifest;
		}

		public bool CheckModVersion(IModData data)
		{
			if (_owmlManifest == null)
			{
				_console.WriteLine("OWML Manifest is null.", MessageType.Error);
				return true;
			}

            // Don't show incompatible OWML version error for disabled mods #369
            if (!data.Config.Enabled)
			{
				return true;
			}

			var owmlVersion = _owmlManifest.Version;

			var splitOwmlVersion = SplitIntoInts(owmlVersion, "OWML");
			var splitModVersion = SplitIntoInts(data.Manifest.OWMLVersion, data.Manifest.UniqueName);

			var (owmlMajor, owmlMinor) = (splitOwmlVersion.Item1, splitOwmlVersion.Item2);
			var (modMajor, modMinor) = (splitModVersion.Item1, splitModVersion.Item2);

			if (splitOwmlVersion == (-1, -1, -1) || splitModVersion == (-1, -1, -1))
			{
				return false;
			}

			var mismatchText = $"Mismatch between OWML version expected by {data.Manifest.UniqueName} and installed OWML version." +
					$"\r\nOWML version expected by {data.Manifest.UniqueName} : {data.Manifest.OWMLVersion}" +
					$"\r\nOWML version installed : {owmlVersion}\r\n";

			if (owmlMajor != modMajor)
			{
				_console.WriteLine(mismatchText + $"As the mismatch affects X.~.~, this mod will not be loaded.", MessageType.Error);
				return false;
			}

			if (owmlMinor < modMinor)
			{
				_console.WriteLine(mismatchText + $"As the the mismatch affects ~.X.~, and the OWML version is lower, the mod will not be loaded.", MessageType.Error);
				return false;
			}

			if (owmlMinor > modMinor)
			{
				_console.WriteLine(mismatchText + $"As the the mismatch affects ~.X.~, and the OWML version is higher, the mod will still load.", MessageType.Debug);
				return true;
			}

			return true;
		}

		(int, int, int) SplitIntoInts(string version, string modname)
		{
			if (string.IsNullOrEmpty(version))
			{
				_console.WriteLine($"Could not read OWML version of \"{modname}\" - No version found. This mod will not be loaded.", MessageType.Error);
				return (-1, -1, -1);
			}

			var split = version.Split('.');
			if (split.Length < 3)
			{
				_console.WriteLine($"Could not read OWML version of \"{modname}\" - Less than 3 digits. This mod will not be loaded.", MessageType.Error);
				return (-1, -1, -1);
			}

			var success = true;
			success &= int.TryParse(split[0], out int major);
			success &= int.TryParse(split[1], out int minor);
			success &= int.TryParse(split[2], out int patch);
			if (!success)
			{
				_console.WriteLine($"Could not read OWML version of \"{modname}\" - Could not parse as digits. This mod will not be loaded.", MessageType.Error);
				return (-1, -1, -1);
			}

			return (major, minor, patch);
		}
	}
}
