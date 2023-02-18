using System;
using System.IO;
using OWML.Common;
using UnityEngine;

namespace OWML.Abstractions
{
	public class ApplicationHelper : IApplicationHelper
	{
		public string DataPath => Path.GetFullPath(Application.dataPath);

		public string Version => Application.version;

		public void AddLogCallback(Action<string, string, LogType> onLogMessageReceived) =>
			Application.logMessageReceived += (condition, trace, type) => onLogMessageReceived(condition, trace, type);
	}
}
