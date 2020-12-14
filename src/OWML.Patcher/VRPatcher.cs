using System.IO;
using OWML.Common;

namespace OWML.Patcher
{
	public class VRPatcher : IVRPatcher
	{
		private readonly IOwmlConfig _owmlConfig;
		private readonly IBinaryPatcher _binaryPatcher;
		private readonly IVRFilePatcher _vrFilePatcher;
		private readonly IModLogger _logger;

		private static readonly string[] PluginFilenames =
		{
			"openvr_api.dll",
			"OVRPlugin.dll"
		};

		public VRPatcher(IOwmlConfig owmlConfig, IBinaryPatcher binaryPatcher, IVRFilePatcher vrFilePatcher, IModLogger logger)
		{
			_owmlConfig = owmlConfig;
			_binaryPatcher = binaryPatcher;
			_vrFilePatcher = vrFilePatcher;
			_logger = logger;
		}

		public void PatchVR(bool enableVR)
		{
			if (enableVR)
			{
				_logger.Log("Patching globalgamemanagers for VR and adding VR plugins");
				_vrFilePatcher.Patch();
				AddPluginFiles();
			}
			else
			{
				_logger.Log("Restoring globalgamemanagers and removing VR plugins");
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
				_logger.Log($"Copying {from} to {to}");
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
					_logger.Log($"Deleting {path}");
					File.Delete(path);
				}
				else
				{
					_logger.Log($"{path} doesn't exist, nothing to delete.");
				}
			}
		}

	}
}
