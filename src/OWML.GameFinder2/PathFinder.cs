using System;
using OWML.Common;

namespace OWML.GameFinder
{
	public class PathFinder : IPathFinder
	{
		private readonly IOwmlConfig _config;
		private readonly IModConsole _writer;

		public PathFinder(IOwmlConfig config, IModConsole writer)
		{
			_config = config;
			_writer = writer;
		}

		public string FindGamePath() =>
			FindPathWith<CurrentPathFinder>() ??
			FindPathWith<SteamGameFinder>() ??
			FindPathWith<EpicGameFinder>() ??
			FindPathWith<DefaultLocationFinder>() ??
			FindPathWith<PromptGameFinder>();

		private string FindPathWith<T>() where T : BaseFinder => 
			((T)Activator.CreateInstance(typeof(T), _config, _writer)).FindGamePath();
	}
}
