using OWML.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OWML.ModHelper.Logging
{
    public static class Output
    {
        public static IModConsole OwmlOutput { get; private set; }
        public static event Action<IModManifest, string> OnWrite;

        public static IModConsole CreateOutput(IOwmlConfig owmlConfig, IModLogger logger, IModManifest manifest)
        {
            IModConsole output;
            if (CommandLineArguments.HasArgument(Constants.ConsolePortArgument))
            {
                output = new ModSocketOutput(logger, manifest);
            }
            else
            {
                output = new ModFileOutput(owmlConfig, logger, manifest);
            }
            if (manifest.Name == "OWML")
            {
                if (OwmlOutput == null)
                {
                    OwmlOutput = output;
                }
                else
                {
                    return OwmlOutput;
                }
            }
            return output;
        }

        internal static void CallWriteCallback(IModManifest manifest, string text)
        {
            OnWrite?.Invoke(manifest, text);
        }
    }
}
