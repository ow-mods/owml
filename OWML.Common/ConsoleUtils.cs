using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OWML.Common
{
    public static class ConsoleUtils
    {
        public static ConsoleColor ConsoleColorFromMessageType(MessageType type)
        {
            switch (type)
            {
                case MessageType.Error:
                    return ConsoleColor.Red;
                case MessageType.Warning:
                    return ConsoleColor.Yellow;
                case MessageType.Success:
                    return ConsoleColor.Green;
                case MessageType.Message:
                    return ConsoleColor.Gray;
                case MessageType.Info:
                    return ConsoleColor.Blue;
                case MessageType.Quit:
                    Environment.Exit(0);
                    break;
            }
            return ConsoleColor.Gray;
        }

        public static void WriteLineWithColor(ConsoleColor color, string line)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(line);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
