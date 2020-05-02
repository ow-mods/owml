using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OWML.Common
{
    public static class CommandLineArguments
    {
        public static string GetArgument(string name)
        {
            var arguments = Environment.GetCommandLineArgs();
            for (var i = 0; i < arguments.Length; i++)
            {
                var argument = arguments[i];
                if (argument == $"-{name}" && arguments.Length > i)
                {
                    return arguments[i + 1];
                }
            }
            return null;
        }

        public static bool HasArgument(string name)
        {
            return GetArgument(name) != null;
        }
    }
}
