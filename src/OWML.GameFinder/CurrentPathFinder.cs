using OWML.Common;

namespace OWML.GameFinder
{
    public class CurrentPathFinder : BaseFinder
    {
        public CurrentPathFinder(IOwmlConfig config, IModConsole writer)
            : base(config, writer)
        {
        }

        public override string FindGamePath()
        {
            if (IsValidGamePath(Config.GamePath))
            {
                return Config.GamePath;
            }
            Writer.WriteLine($"Current game path is not valid: {Config.GamePath}", MessageType.Warning);
            return null;
        }

    }
}
