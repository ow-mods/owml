using System;
using System.Linq;
using OWML.Common;

namespace OWML.ModHelper.Logging
{
    public class OutputWriter : IModConsole
    {
        [Obsolete("Use OutputWriter.Writeline( MessageType type, string line) instead")]
        public void WriteLine(string line)
        {
            MessageType type;
            if (line.ToLower().Contains("error") || line.ToLower().Contains("exception"))
            {
                type = MessageType.Error;
            }
            else if (line.ToLower().Contains("warning") || line.ToLower().Contains("disabled"))
            {
                type = MessageType.Warning;
            }
            else if (line.ToLower().Contains("success"))
            {
                type = MessageType.Success;
            }
            else
            {
                type = MessageType.Message;
            }
            WriteLine(type, line);
        }

        [Obsolete("Use OutputWriter.Writeline(params object[] objects, MessageType type) instead")]
        public void WriteLine(params object[] objects)
        {
            WriteLine(string.Join(" ", objects.Select(o => o.ToString()).ToArray()));
        }

        public void WriteLine(MessageType type, string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return;
            }

            ConsoleUtils.WriteByType(type, line);
        }

        public void WriteLine(MessageType type, params object[] objects)
        {
            WriteLine(type, string.Join(" ", objects.Select(o => o.ToString()).ToArray()));
        }
    }
}
