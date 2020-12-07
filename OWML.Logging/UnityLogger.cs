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
        private readonly IApplicationHelper _appHelper;

        public UnityLogger(IModSocket socket, IApplicationHelper appHelper)
        {
            _socket = socket;
            _appHelper = appHelper;
        }

        public void Start()
        {
            _appHelper.AddLogCallback(OnLogMessageReceived);
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
