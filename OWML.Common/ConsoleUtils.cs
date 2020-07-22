using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OWML.Common
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
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case MessageType.Quit:
                    Environment.Exit(0);
                    break;
            }
            Console.WriteLine(line);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
