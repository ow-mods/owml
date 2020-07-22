using OWML.Common;
using System;

namespace OWML.ModHelper
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

        public static MessageType ContentsToType(string line)
        {
            if (line.ToLower().Contains("error") || line.ToLower().Contains("exception"))
            {
                return MessageType.Error;
            }
            else if (line.ToLower().Contains("warning") || line.ToLower().Contains("disabled"))
            {
                return MessageType.Warning;
            }
            else if (line.ToLower().Contains("success"))
            {
                return MessageType.Success;
            }
            else
            {
                return MessageType.Message;
            }
        }
    }
}
