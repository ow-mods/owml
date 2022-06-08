using System;
using System.IO;
using Newtonsoft.Json;

namespace OWML.Utils
{
	public static class JsonHelper
	{
		[Obsolete]
		public static T LoadJsonObject<T>(string path)
		{
			return LoadJsonObject<T>(path, true, null);
		}

		public static T LoadJsonObject<T>(string path, bool fixBackslashes, JsonSerializerSettings settings = null)
		{
			if (!File.Exists(path))
			{
				return default;
			}

			var json = File.ReadAllText(path);

			if (fixBackslashes)
			{
				json = json
					.Replace("\\\\", "/")
					.Replace("\\", "/");
			}

			try
			{
				return settings != null
					? JsonConvert.DeserializeObject<T>(json, settings)
					: JsonConvert.DeserializeObject<T>(json);
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
