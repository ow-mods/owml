using System;
using System.Linq;
using OWML.Common;

namespace OWML.ModHelper
{
    public class OutputWriter : IModConsole
    {
        [Obsolete("Use OutputWriter.Writeline(params object[] objects, MessageType type) instead")]
        public void WriteLine(params object[] objects)
        {
            WriteLine(string.Join(" ", objects.Select(o => o.ToString()).ToArray()));
        }

        public void WriteLine(string line, MessageType type = MessageType.Message)
        {
            if (string.IsNullOrEmpty(line))
            {
                return;
            }

            ConsoleUtils.WriteByType(type, line);
        }
    }
}
