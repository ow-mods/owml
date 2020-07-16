using OWML.Common;


namespace OWML.ModHelper
{
    public class OutputFactory
    {
        public static IModConsole CreateOutput(IOwmlConfig owmlConfig, IModLogger logger, IModManifest manifest)
        {
            if (owmlConfig.SocketPort != -1)
            {
                return new ModSocketOutput(owmlConfig, logger, manifest);
            }

            if (logger == null)
            {
                return new OutputWriter();
            }

            return new ModFileOutput(owmlConfig, logger, manifest);
        }
    }
}
