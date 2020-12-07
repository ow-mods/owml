using System;
using OWML.Common.Interfaces;
using UnityEngine;

namespace OWML.UnityAbstractions
{
    public class ApplicationHelper : IApplicationHelper
    {
        public string DataPath => Application.dataPath;

        public string Version => Application.version;

        public void AddLogCallback(Action<string, string, LogType> onLogMessageReceived) => 
            Application.logMessageReceived += (condition, trace, type) => onLogMessageReceived(condition, trace, type);
    }
}
