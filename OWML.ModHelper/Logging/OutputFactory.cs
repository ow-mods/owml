using OWML.Common;


namespace OWML.ModHelper
{
    public class OutputFactory
    {
        public static IModConsole CreateOutput(IOwmlConfig owmlConfig, IModLogger logger, IModManifest manifest, bool consolePortPresent, bool listenToUnity)
        {
            if (consolePortPresent)
            {
                return new ModSocketOutput(owmlConfig, logger, manifest, listenToUnity);
            }

            return new OutputWriter();
        }
    }
}
