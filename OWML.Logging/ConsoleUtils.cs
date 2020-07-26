using System;
using OWML.Common;

namespace OWML.Logging
{
    public static class ConsoleUtils
    {
        public static void WriteByType(MessageType type, string line)
        {
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
                case MessageType.Message:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case MessageType.Info:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
            }
            Console.WriteLine(line);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
