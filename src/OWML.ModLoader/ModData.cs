using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using OWML.Common;

namespace OWML.ModLoader
{
	public class ModData : IModData
	{
		public IModManifest Manifest { get; }

		public IModConfig Config { get; private set; }

		public IModConfig DefaultConfig { get; }

		public IModStorage Storage { get; }

		public bool RequireReload => Config.Enabled != _configSnapshot.Enabled;

		public bool RequireVR => Manifest.RequireVR;

		public bool Enabled => Config != null && Config.Enabled ||
							   Config == null && DefaultConfig.Enabled;

		private IModConfig _configSnapshot;


		public ModData(IModManifest manifest, IModConfig config, IModConfig defaultConfig, IModStorage storage)
		{
			Manifest = manifest;
			Config = config;
			DefaultConfig = defaultConfig;
			Storage = storage;

			UpdateSnapshot();
		}

		public void UpdateSnapshot() =>
			_configSnapshot = Config != null ? Config.Copy() : DefaultConfig.Copy();

		public void ResetConfigToDefaults() =>
			Config = DefaultConfig.Copy();

		public bool FixConfigs()
		{
			var settingsChanged = false;
			if (Config == null)
			{
				Config = DefaultConfig.Copy();
			}
			else if (DefaultConfig != null)
			{
				settingsChanged = !MakeConfigConsistentWithDefault();
			}

			Storage.Save(Config, Constants.ModConfigFileName);
			UpdateSnapshot();
			return settingsChanged;
		}

		private bool MakeConfigConsistentWithDefault()
		{
			var wasCompatible = true;
			var toRemove = Config.Settings.Keys.Except(DefaultConfig.Settings.Keys).ToList();
			toRemove.ForEach(key => Config.Settings.Remove(key));

			var keysCopy = Config.Settings.Keys.ToList();
			foreach (var key in keysCopy)
			{
				if (!IsSettingSameType(Config.Settings[key], DefaultConfig.Settings[key]))
				{
					wasCompatible = TryUpdate(key, Config.Settings[key], DefaultConfig.Settings[key]) && wasCompatible;
				}
				else if (DefaultConfig.Settings[key] is JObject objectValue && objectValue["type"].ToString() == "selector")
				{
					wasCompatible = UpdateSelector(key, Config.Settings[key], objectValue) && wasCompatible;
				}
			}

			AddMissingDefaults(DefaultConfig);
			return wasCompatible;
		}

		private bool UpdateSelector(string key, object userSetting, JObject modSetting)
		{
			var options = modSetting["options"].ToObject<List<string>>();
			var userString = userSetting is JObject objectValue ? (string)objectValue["value"] : Convert.ToString(userSetting);
			Config.Settings[key] = modSetting;
			var isInOptions = options.Contains(userString);
			if (isInOptions)
			{
				Config.SetSettingsValue(key, userString);
			}
			return isInOptions;
		}

		private void AddMissingDefaults(IModConfig defaultConfig) =>
			defaultConfig.Settings.Where(s => !Config.Settings.ContainsKey(s.Key)).ToList()
				.ForEach(setting => Config.Settings.Add(setting.Key, setting.Value));

		private bool TryUpdate(string key, object userSetting, object modSetting)
		{
			var userValue = Config.GetSettingsValue<object>(key);
			if (userValue is JValue userJValue)
			{
				userValue = userJValue.Value;
			}
			Config.Settings[key] = modSetting;

			if (IsNumber(userSetting) && IsNumber(modSetting))
			{
				Config.SetSettingsValue(key, Convert.ToDouble(userValue));
				return true;
			}

			if (IsBoolean(userSetting) && IsBoolean(modSetting))
			{
				Config.SetSettingsValue(key, Convert.ToBoolean(userValue));
				return true;
			}
			return false;
		}

		private bool IsNumber(object setting)
		{
			try
			{
				return setting is JObject settingObject
					? settingObject["type"].ToString() == "slider"
					: new[] { typeof(long), typeof(int), typeof(float), typeof(double) }.Contains(setting.GetType());
			}
			catch (Exception)
			{
				return false;
			}
		}

		private bool IsBoolean(object setting)
		{
			try
			{
				return setting is JObject settingObject
					? settingObject["type"].ToString() == "toggle"
					: setting is bool;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private bool IsSettingSameType(object settingValue1, object settingValue2)
		{
			try
			{
				return settingValue1.GetType() == settingValue2.GetType() &&
					   (settingValue1 is not JObject obj1 ||
						settingValue2 is not JObject obj2 ||
						(string)obj1["type"] == (string)obj2["type"]);
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
