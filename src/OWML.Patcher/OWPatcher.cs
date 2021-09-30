using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dnlib.DotNet.Emit;
using dnpatch;
using OWML.Common;

namespace OWML.Patcher
{
	public class OWPatcher : IOWPatcher
	{
		private readonly IOwmlConfig _owmlConfig;
		private readonly IModConsole _writer;

		private const string PatchClass = "PermanentManager";
		private const string PatchMethod = "Awake";

		public OWPatcher(IOwmlConfig owmlConfig, IModConsole writer)
		{
			_owmlConfig = owmlConfig;
			_writer = writer;
		}

		public void PatchGame()
		{
			var filesToCopy = new[] {
				"OWML.ModLoader.dll",
				"winhttp.dll",
				"doorstop_config.ini",
				"OWML.Common.dll",
				"OWML.ModHelper.dll",
				"OWML.ModHelper.Events.dll",
				"OWML.ModHelper.Assets.dll",
				"OWML.ModHelper.Input.dll",
				"OWML.ModHelper.Menus.dll",
				"OWML.ModHelper.Interaction.dll",
				"OWML.Logging.dll",
				"OWML.Utils.dll",
				"OWML.Abstractions.dll",
				"Newtonsoft.Json.dll",
				"System.Runtime.Serialization.dll",
				"0Harmony.dll",
				"NAudio-Unity.dll",
				"Microsoft.Practices.Unity.dll",
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
	}
}
