﻿namespace OWML.Common
{
	public interface IOwmlConfig
	{
		string GamePath { get; set; }

		string ManagedPath { get; }

		string PluginsPath { get; }

		string DataPath { get; }

		string ExePath { get; }

		string OWMLPath { get; set; }

		string ModsPath { get; }

		string LogsPath { get; }

		bool DebugMode { get; set; }

		bool ForceExe { get; set; }

		int SocketPort { get; set; }
	}
}
