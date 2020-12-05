using System.Linq;
using OWML.Common.Enums;
using OWML.Common.Interfaces;
using UnityEngine;

namespace OWML.Logging
{
    public class UnityLogger : IUnityLogger
    {
        private readonly LogType[] _relevantTypes = {
            LogType.Error,
            LogType.Exception
        };

        private readonly IModSocket _socket;

        public UnityLogger(IModSocket socket)
        {
            _socket = socket;
        }

        public void Start()
        {
            Application.logMessageReceived += OnLogMessageReceived;
        }

        private void OnLogMessageReceived(string message, string stackTrace, LogType type)
        {
            if (!_relevantTypes.Contains(type))
            {
                return;
            }
            var line = $"{message}. Stack trace: {stackTrace?.Trim()}";
            _socket.WriteToSocket(new ModSocketMessage
            {
                Type = MessageType.Error,
                Message = line,
                SenderName = "Unity",
                SenderType = type.ToString()
            });
        }

    }
}
