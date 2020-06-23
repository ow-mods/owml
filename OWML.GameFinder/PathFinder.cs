using System;
using OWML.Common;

namespace OWML.GameFinder
{
    public class PathFinder
    {
        private readonly IOwmlConfig _config;
        private readonly IModConsole _writer;

        public PathFinder(IOwmlConfig config, IModConsole writer)
        {
            _config = config;
            _writer = writer;
        }

        public string FindGamePath()
        {
            return FindPathWith<CurrentPathFinder>() ??
                   FindPathWith<DefaultLocationFinder>() ??
                   FindPathWith<EpicGameFinder>() ??
                   FindPathWith<SteamGameFinder>() ??
                   FindPathWith<PromptGameFinder>();
        }

        private string FindPathWith<T>() where T : BaseFinder
        {
            var gameFinder = (T)Activator.CreateInstance(typeof(T), _config, _writer);
            return gameFinder.FindGamePath();
        }
    }
}
