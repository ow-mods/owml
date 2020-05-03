using OWML.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OWML.ModHelper.Logging
{
    public static class Output
    {
        private static IModConsole _owmlOutput;

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
                if (_owmlOutput == null)
                {
                    _owmlOutput = output;
                }
                else
                {
                    return _owmlOutput;
                }
            }
            return output;
        }

        internal static void SetOwmlOutput(IModConsole modConsole)
        {
            _owmlOutput = modConsole;
        }

        public static IModConsole GetOwmlOutput()
        {
            return _owmlOutput;
        }
    }
}
