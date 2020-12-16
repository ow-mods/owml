using System;
using UnityEngine;

namespace OWML.Common
{
	public interface IApplicationHelper
	{
		string DataPath { get; }

		string Version { get; }

		void AddLogCallback(Action<string, string, LogType> onLogMessageReceived);
	}
}