using System;

namespace OWML.ModHelper
{
    public static class CommandLineArguments
    {
        public static string GetArgument(string name)
        {
            var arguments = Environment.GetCommandLineArgs();
            var keyIndex = Array.IndexOf(arguments, $"-{name}");
            if (keyIndex == -1 || keyIndex >= arguments.Length - 1)
            {
                return null;
            }
            return arguments[keyIndex + 1];
        }

        public static bool HasArgument(string name)
        {
            return GetArgument(name) != null;
        }
    }
}
