using System.IO;
using OWML.Common;

namespace OWML.GameFinder
{
	public abstract class BaseFinder
	{
		private const string ManagedPath = "OuterWilds_Data/Managed";
		private const string ExePath = "OuterWilds.exe";

		protected IOwmlConfig Config;
		protected IModConsole Writer;

		protected BaseFinder(IOwmlConfig config, IModConsole writer)
		{
			Config = config;
			Writer = writer;
		}

		public abstract string FindGamePath();

		protected bool IsValidGamePath(string gamePath)
		{
			return !string.IsNullOrEmpty(gamePath) &&
				   Directory.Exists(gamePath) &&
				   Directory.Exists($"{gamePath}/{ManagedPath}") &&
				   File.Exists($"{gamePath}/{ExePath}");
		}
	}
}