﻿using Newtonsoft.Json;
using OWML.Common;
using OWML.Utils;
using System;

namespace OWML.ModHelper
{
	public class ModStorage : IModStorage
	{
		private readonly IModManifest _manifest;
		private readonly IModConsole _console;

		public ModStorage(IModManifest manifest, IModConsole console)
		{
			_manifest = manifest;
			_console = console;
		}

		[Obsolete]
		public T Load<T>(string filename)
		{
			return Load<T>(filename, true, null);
		}

		public T Load<T>(string filename, bool fixBackslashes, JsonSerializerSettings settings = null)
		{
			var path = _manifest.ModFolderPath + filename;
			_console.WriteLine($"Loading config from {path}", MessageType.Debug);
			return JsonHelper.LoadJsonObject<T>(path, fixBackslashes, settings);
		}

		public void Save<T>(T obj, string filename)
		{
			var path = _manifest.ModFolderPath + filename;
			_console.WriteLine($"Saving config to {path}", MessageType.Debug);
			JsonHelper.SaveJsonObject(path, obj);
		}
	}
}
