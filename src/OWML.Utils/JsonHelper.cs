using Newtonsoft.Json;
using System;
using System.IO;

namespace OWML.Utils
{
	public static class JsonHelper
	{
		public static T LoadJsonObject<T>(string path)
		{
			if (!File.Exists(path))
			{
				return default;
			}

			var json = File.ReadAllText(path)
				.Replace("\\\\", "/")
				.Replace("\\", "/");

			try
			{
				return JsonConvert.DeserializeObject<T>(json);
			}
			catch (Exception)
			{
				return default;
			}
		}

		public static void SaveJsonObject<T>(string path, T obj) =>
			File.WriteAllText(path, JsonConvert.SerializeObject(obj, Formatting.Indented));
	}
}
