using System.Linq;
using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper
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
                ModConsole.Instance.WriteLine(line, MessageType.Error);
            }
        }

    }
}
