using OWML.Common;
using UnityEngine;

namespace OWML.Logging
{
	public class UnityLogger : IUnityLogger
	{
		private readonly IApplicationHelper _appHelper;
		private readonly IModConsole _console;
		private readonly IOwmlConfig _config;

		public UnityLogger(IApplicationHelper appHelper, IModConsole console, IOwmlConfig config)
		{
			_appHelper = appHelper;
			_console = console;
			_config = config;
		}

		public void Start() =>
			_appHelper.AddLogCallback(OnLogMessageReceived);

		private void OnLogMessageReceived(string message, string stackTrace, LogType type)
		{
			if (type != LogType.Error
			    && type != LogType.Exception 
			    && !_config.DebugMode)
			{
				return;
			}

			var line = (stackTrace is not null or "") 
				? $"{message} | Stacktrace: {stackTrace?.Trim()}" 
				: $"{message}";

			var messageType = type switch
			{
				LogType.Error => MessageType.Error,
				LogType.Exception => MessageType.Error,
				LogType.Warning => MessageType.Warning,
				_ => MessageType.Debug
			};

			_console.WriteLine(line, messageType, "Unity");
		}
	}
}
