using System.Linq;
using OWML.Common;
using UnityEngine;

namespace OWML.Logging
{
    public class UnityLogger
    {
        private readonly LogType[] _relevantTypes = {
            LogType.Error,
            LogType.Exception
        };

        public void Start()
        {
            Application.logMessageReceived += OnLogMessageReceived;
        }

        private void OnLogMessageReceived(string message, string stackTrace, LogType type)
        {
            if (_relevantTypes.Contains(type))
            {
                var line = $"{message}. Stack trace: {stackTrace?.Trim()}";
                ModConsole.OwmlConsole.WriteLine(line, MessageType.Error);
            }
        }

    }
}
