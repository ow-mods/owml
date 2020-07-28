using OWML.Common;
using System;
using System.Collections.Generic;

namespace OWML.ModHelper
{
    public static class ConsoleUtils
    {
        public static void WriteByType(MessageType type, string line)
        {
            Console.ForegroundColor = new Dictionary<MessageType, ConsoleColor>
            {
                { MessageType.Error, ConsoleColor.Red },
                { MessageType.Warning, ConsoleColor.Yellow },
                { MessageType.Success, ConsoleColor.Green },
                { MessageType.Message, ConsoleColor.Gray },
                { MessageType.Info, ConsoleColor.Cyan }
            }[type];
            Console.WriteLine(line);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
