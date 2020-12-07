using System;
using UnityEngine;

namespace OWML.Common.Interfaces
{
    public interface IApplicationHelper
    {
        string DataPath { get; }

        string Version { get; }

        void AddLogCallback(Action<string, string, LogType> onLogMessageReceived);
    }
}