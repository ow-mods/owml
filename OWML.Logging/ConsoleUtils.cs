using System;
using System.Collections.Generic;
using OWML.Common.Enums;

namespace OWML.Logging
{
    public static class ConsoleUtils
    {
        private static Dictionary<MessageType, ConsoleColor> _messageTypeColors = new Dictionary<MessageType, ConsoleColor> {
            { MessageType.Error, ConsoleColor.Red },
            { MessageType.Warning, ConsoleColor.Yellow },
            { MessageType.Success, ConsoleColor.Green },
            { MessageType.Message, ConsoleColor.Gray },
            { MessageType.Info, ConsoleColor.Cyan },
            { MessageType.Fatal, ConsoleColor.Magenta }
        };

        public static void WriteByType(MessageType type, string line)
        {
            if (_messageTypeColors.ContainsKey(type))
            {
                Console.ForegroundColor = _messageTypeColors[type];
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            Console.WriteLine(line);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
