using System;
using System.IO;
using System.Linq;
using OWML.Common;

namespace OWML.Logging
{
	public class ModLogger : IModLogger
	{
		private readonly IModManifest _manifest;
		private static string _logFileName;
		private static string _latestFileName;

		public ModLogger(IOwmlConfig config, IModManifest manifest)
		{
			_manifest = manifest;
			_logFileName = $"{config.LogsPath}/OWML.Log.{config.LoadTime:yyyy-MM-ddTHH.mm.ss}.txt";

			if (!Directory.Exists(config.LogsPath))
			{
				Directory.CreateDirectory(config.LogsPath);
			}

			_latestFileName = $"{config.LogsPath}/latest.txt";
		}

		[Obsolete("Use ModHelper.Console.WriteLine with messageType = Debug instead.")]
		public void Log(string s) =>
			LogInternal($"[{_manifest.Name}]: {s}");

		[Obsolete("Use ModHelper.Console.WriteLine with messageType = Debug instead.")]
		public void Log(params object[] objects) =>
			Log(string.Join(" ", objects.Select(o => o.ToString()).ToArray()));

		private static void LogInternal(string message)
		{
			var text = $"{DateTime.Now}: {message}{Environment.NewLine}";
			File.AppendAllText(_logFileName, text);
			File.AppendAllText(_latestFileName, text);
		}
	}
}
