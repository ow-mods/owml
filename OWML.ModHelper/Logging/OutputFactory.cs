using OWML.Common;


namespace OWML.ModHelper
{
    public class OutputFactory
    {
        public static IModConsole CreateOutput(IOwmlConfig owmlConfig, IModLogger logger, IModManifest manifest, bool consolePortPresent)
        {
            if (consolePortPresent)
            {
                return new ModSocketOutput(owmlConfig, logger, manifest);
            }

            return new OutputWriter();
        }
    }
}
