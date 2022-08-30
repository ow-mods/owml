using OWML.Common.Enums;

namespace OWML.Common
{
	public interface IModManifest
	{
		string Filename { get; }

		string Patcher { get; }

		string Author { get; }

		string Name { get; }

		string Version { get; }

		string OWMLVersion { get; }

		string AssemblyPath { get; }

		string PatcherPath { get; }

		string UniqueName { get; }

		string ModFolderPath { get; set; }

		string[] Dependencies { get; }

		bool PriorityLoad { get; }

		string MinGameVersion { get; }

		string MaxGameVersion { get; }

		bool RequireLatestVersion { get; }

		GameVendor[] IncompatibleVendors { get; }
	}
}
