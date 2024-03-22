﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper
{
	public class ModDefaultConfig : IModDefaultConfig
	{
		[JsonProperty("enabled")]
		public bool Enabled { get; set; } = true;

		[JsonProperty("settings")]
		public Dictionary<string, object> Settings { get; set; } = new();

		public T GetSettingsValue<T>(string key)
		{
			if (Settings.ContainsKey(key))
			{
				return GetSettingsValue<T>(key, Settings[key]);
			}

			Debug.LogError($"Setting not found: {key}");
			return default;
		}

		private T GetSettingsValue<T>(string key, object setting)
		{
			try
			{
				var type = typeof(T);
				var value = setting is JObject objectValue ? objectValue["value"] : setting;
				return type.IsEnum ? ConvertToEnum<T>(value) : (T)Convert.ChangeType(value, type);
			}
			catch (Exception ex)
			{
				Debug.LogError($"Error when getting setting {key}: {ex}");
				return default;
			}
		}

		private T ConvertToEnum<T>(object value)
		{
			if (value is float || value is double)
			{
				var floatValue = Convert.ToDouble(value);
				return (T)(object)(long)Math.Round(floatValue);
			}

			if (value is int || value is long)
			{
				return (T)value;
			}

			var valueString = Convert.ToString(value);

			try
			{
				return (T)Enum.Parse(typeof(T), valueString, true);
			}
			catch (ArgumentException ex)
			{
				Debug.LogError($"Can't convert {valueString} to enum {typeof(T)}: {ex}");
				return default;
			}
		}

		public IModConfig Copy() =>
			new ModConfig
			{
				Enabled = Enabled,
				Settings = new Dictionary<string, object>(Settings)
			};
	}
}
