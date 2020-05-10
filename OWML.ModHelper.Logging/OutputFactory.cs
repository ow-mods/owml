using OWML.Common;


namespace OWML.ModHelper.Logging
{
    public class OutputFactory
    {
        public static IModConsole CreateOutput(IOwmlConfig owmlConfig, IModLogger logger, IModManifest manifest)
        {
            if (CommandLineArguments.HasArgument(Constants.ConsolePortArgument))
            {
                return new ModSocketOutput(owmlConfig, logger, manifest);
            }
            else if (logger == null)
            {
                return new OutputWriter();
            }
            else
            {
                return new ModFileOutput(owmlConfig, logger, manifest);
            }
        }
    }
}
