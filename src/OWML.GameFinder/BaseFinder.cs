using System.IO;
using OWML.Common;

namespace OWML.GameFinder
{
	public abstract class BaseFinder
	{
		private readonly string ManagedPath = Path.Combine("OuterWilds_Data", "Managed");
		private const string ExePath = "OuterWilds.exe";
		private readonly string SpacedManagedPath = Path.Combine("Outer Wilds_Data", "Managed");
		private const string SpacedExePath = "Outer Wilds.exe";

		protected IOwmlConfig Config;
		protected IModConsole Writer;

		protected BaseFinder(IOwmlConfig config, IModConsole writer)
		{
			Config = config;
			Writer = writer;
		}

		public abstract string FindGamePath();

		protected bool IsValidGamePath(string gamePath) =>
			!string.IsNullOrEmpty(gamePath) &&
			Directory.Exists(gamePath) &&
			(HasGameFiles(gamePath) || HasSpacedGameFiles(gamePath));

		private bool HasGameFiles(string gamePath) => 
			Directory.Exists(Path.Combine(gamePath, ManagedPath)) && 
			File.Exists(Path.Combine(gamePath, ExePath));

		private bool HasSpacedGameFiles(string gamePath) => 
			Directory.Exists(Path.Combine(gamePath, SpacedManagedPath)) && 
			File.Exists(Path.Combine(gamePath, SpacedExePath));
	}
}