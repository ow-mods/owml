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
            WriteLine(MessageType.Log, line);
        }

        [Obsolete("Use OutputWriter.Writeline(params object[] objects, MessageType type) instead")]
        public void WriteLine(params object[] objects)
        {
            WriteLine(string.Join(" ", objects.Select(o => o.ToString()).ToArray()));
        }

        public void WriteLine(MessageType type, string line)
        {
            if(string.IsNullOrEmpty(line))
            {
                return;
            }
            switch (type)
            {
                case MessageType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case MessageType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case MessageType.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case MessageType.Log:
                    break;
            }
            Console.WriteLine(line);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void WriteLine(MessageType type, params object[] objects)
        {
            WriteLine(type, string.Join(" ", objects.Select(o => o.ToString()).ToArray()));
        }
    }
}
