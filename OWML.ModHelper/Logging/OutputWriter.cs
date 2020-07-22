using System;
using System.Linq;
using OWML.Common;

namespace OWML.ModHelper
{
    public class OutputWriter : IModConsole
    {
        [Obsolete]
        public void WriteLine(params object[] objects)
        {
            WriteLine(string.Join(" ", objects.Select(o => o.ToString()).ToArray()));
        }

        [Obsolete]
        public void WriteLine(string line)
        {
            WriteLine(MessageType.Message, line);
        }

        public void WriteLine(MessageType type, string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return;
            }

            ConsoleUtils.WriteByType(type, line);
        }
    }
}
