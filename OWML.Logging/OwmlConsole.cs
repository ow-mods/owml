using OWML.Common;

namespace OWML.Logging
{
    public static class OwmlConsole
    {
        private static IModConsole _console;

        public static void Init(IModConsole console)
        {
            _console = console;
        }

        public static void WriteLine(string line)
        {
            WriteLine(line, MessageType.Message);
        }

        public static void WriteLine(string line, MessageType type)
        {
            _console?.WriteLine(line, type);
        }
    }
}
