using System.IO;
using OWML.Common;

namespace OWML.Patcher
{
	public class VRPatcher : IVRPatcher
	{
		private readonly IOwmlConfig _owmlConfig;
		private readonly IBinaryPatcher _binaryPatcher;
		private readonly IVRFilePatcher _vrFilePatcher;
		private readonly IModConsole _console;

		private static readonly string[] PluginFilenames =
		{
			"openvr_api.dll",
			"OVRPlugin.dll"
		};

		public VRPatcher(IOwmlConfig owmlConfig, IBinaryPatcher binaryPatcher, IVRFilePatcher vrFilePatcher, IModConsole console)
		{
			_owmlConfig = owmlConfig;
			_binaryPatcher = binaryPatcher;
			_vrFilePatcher = vrFilePatcher;
			_console = console;
		}

		public void PatchVR(bool enableVR)
		{
			if (enableVR)
			{
				_console.WriteLine("Patching globalgamemanagers for VR and adding VR plugins", MessageType.Debug);
				_vrFilePatcher.Patch();
				AddPluginFiles();
			}
			else
			{
				_console.WriteLine("Restoring globalgamemanagers and removing VR plugins", MessageType.Debug);
				_binaryPatcher.RestoreFromBackup();
				RemovePluginFiles();
			}
		}

		private void AddPluginFiles()
		{
			foreach (var filename in PluginFilenames)
			{
				var from = $"{_owmlConfig.OWMLPath}lib/{filename}";
				var to = $"{_owmlConfig.PluginsPath}/{filename}";
				_console.WriteLine($"Copying {from} to {to}", MessageType.Debug);
				File.Copy(from, to, true);
			}
		}

		private void RemovePluginFiles()
		{
			foreach (var filename in PluginFilenames)
			{
				var path = $"{_owmlConfig.PluginsPath}/{filename}";
				if (File.Exists(path))
				{
					_console.WriteLine($"Deleting {path}", MessageType.Debug);
					File.Delete(path);
				}
				else
				{
					_console.WriteLine($"{path} doesn't exist, nothing to delete.", MessageType.Debug);
				}
			}
		}
	}
}
