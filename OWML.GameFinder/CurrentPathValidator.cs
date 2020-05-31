using OWML.Common;

namespace OWML.GameFinder
{
    public class CurrentPathValidator : BaseFinder
    {
        public CurrentPathValidator(IOwmlConfig config, IModConsole writer) : base(config, writer)
        {
        }

        public override string FindGamePath()
        {
            if (IsValidGamePath(Config.GamePath))
            {
                return Config.GamePath;
            }
            Writer.WriteLine($"Current game path is not valid: {Config.GamePath}");
            return null;
        }

    }
}
