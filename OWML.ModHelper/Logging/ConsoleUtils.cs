using OWML.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OWML.ModHelper
{
    public static class ConsoleUtils
    {
        public static readonly List<String> ErrorList = new List<String> {
            "error",
            "exception"
        };

        public static readonly List<String> WarningList = new List<String> {
            "warning",
            "disabled"
        };

        public static readonly List<String> SuccessList = new List<String> {
            "success"
        };

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
            if (ErrorList.Contains(line.ToLower()))
            {
                return MessageType.Error;
            }
            if (WarningList.Contains(line.ToLower()))
            {
                return MessageType.Warning;
            }
            if (SuccessList.Contains(line.ToLower()))
            {
                return MessageType.Success;
            }
            return MessageType.Message;
        }
    }
}
