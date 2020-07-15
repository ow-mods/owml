using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OWML.Common;

namespace OWML.ModHelper
{
    public class ModConfig : IModConfig
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; } = true;

        [JsonProperty("requireVR")]
        public bool RequireVR { get; set; } = false;

        [JsonProperty("settings")]
        public Dictionary<string, object> Settings { get; set; } = new Dictionary<string, object>();

        public T GetSettingsValue<T>(string key)
        {
            if (!Settings.ContainsKey(key))
            {
                ModConsole.Instance.WriteLine("Error: setting not found: " + key);
                return default;
            }

            var value = Settings[key];

            try
            {
                var val = value is JObject obj ? obj["value"] : value;
                return (T)Convert.ChangeType(val, typeof(T));
            }
            catch (InvalidCastException)
            {
                ModConsole.Instance.WriteLine($"Error when converting setting {key} of type {value.GetType()} to type {typeof(T)}");
                return default;
            }
        }

        public void SetSettingsValue(string key, object val)
        {
            if (!Settings.ContainsKey(key))
            {
                ModConsole.Instance.WriteLine("Error: setting not found: " + key);
                return;
            }

            var value = Settings[key];

            if (value is JObject obj)
            {
                obj["value"] = "" + val;
            }
            else
            {
                Settings[key] = val;
            }
        }

        [Obsolete("Use GetSettingsValue instead")]
        public T GetSetting<T>(string key)
        {
            return GetSettingsValue<T>(key);
        }

        private void AddMissingDefaults(IModConfig defaultConfig)
        {
            foreach (var defaultSetting in defaultConfig.Settings)
            {
                if (!Settings.ContainsKey(defaultSetting.Key))
                {
                    Settings.Add(defaultSetting.Key, defaultSetting.Value);
                }
            }
        }

        public void MakeConfigConsistentWithDefaults(IModConfig defaultConfig)
        {
            if (defaultConfig == null)
            {
                return;
            }

            AddMissingDefaults(defaultConfig);

            var settingsCopy = new Dictionary<string, object>(Settings);
            foreach (var setting in Settings)
            {
                if (!defaultConfig.Settings.ContainsKey(setting.Key))
                {
                    settingsCopy.Remove(setting.Key);
                    continue;
                }
                else if (!IsSettingSameType(setting.Value, defaultConfig.Settings[setting.Key]))
                {
                    settingsCopy[setting.Key] = defaultConfig.Settings[setting.Key];
                    continue;
                }
            }
            Settings = settingsCopy;
        }

        private bool IsSettingSameType(object settingValue1, object settingValue2)
        {
            if (settingValue1.GetType() != settingValue2.GetType())
            {
                return false;
            }
            if (settingValue1 is JObject obj1 && settingValue2 is JObject obj2)
            {
                if ((string)obj1["type"] != (string)obj2["type"])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
