using OWML.Common;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OWML.Patcher
{
	public class OWPatcher : IOWPatcher
	{
		private readonly IOwmlConfig _owmlConfig;
		private readonly IModConsole _writer;
		private readonly IModConsole _console;

		public OWPatcher(IOwmlConfig owmlConfig, IModConsole writer, IModConsole console)
		{
			_owmlConfig = owmlConfig;
			_writer = writer;
			_console = console;
		}

		public void PatchGame()
		{
			_console.WriteLine("PatchGame");
			CopyOWMLFiles();
			CopyDoorstopDll();
			CopyDoorstopConfig();
		}

		private void CopyOWMLFiles()
		{
			var filesToCopy = new[] {
				"OWML.Common.dll",
				"OWML.Entry.dll",
				"OWML.ModHelper.dll",
				"OWML.Logging.dll",
				"OWML.Loader.dll",
				"OWML.Utils.dll",
				"OWML.Abstractions.dll",
				"NAudio-Unity.dll",
				Constants.OwmlManifestFileName,
				Constants.OwmlConfigFileName,
				Constants.OwmlDefaultConfigFileName
			};

			var uncopiedFiles = new List<string>();
			foreach (var filename in filesToCopy)
			{
				try
				{
					File.Copy(filename, $"{_owmlConfig.ManagedPath}/{filename}", true);
				}
				catch
				{
					uncopiedFiles.Add(filename);
				}
			}

			if (uncopiedFiles.Any())
			{
				_writer.WriteLine("Warning - Failed to copy the following files:", MessageType.Warning);
				uncopiedFiles.ForEach(file => _writer.WriteLine($"* {file}", MessageType.Warning));
			}
		}

		private void CopyDoorstopDll()
		{
			try
			{
				File.Copy("winhttp.dll", $"{_owmlConfig.GamePath}/winhttp.dll", true);
			}
			catch (FileNotFoundException)
			{
				_console.WriteLine("Error - Could not find winhttp.dll!", MessageType.Error);
			}
		}

		private void CopyDoorstopConfig()
		{
			try
			{
				File.Copy("doorstop_config.ini", $"{_owmlConfig.GamePath}/doorstop_config.ini", true);
			}
			catch (FileNotFoundException)
			{
				_console.WriteLine("Error - Could not find doorstop_config.ini!", MessageType.Error);
			}
		}
	}
}
