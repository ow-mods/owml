using System;
using System.Collections.Generic;
using System.Linq;
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

        public void SetSettingsValue(string key, object value)
        {
            if (!Settings.ContainsKey(key))
            {
                ModConsole.Instance.WriteLine("Error: setting not found: " + key);
                return;
            }

            if (Settings[key] is JObject setting)
            {
                setting["value"] = JToken.FromObject(value);
            }
            else
            {
                Settings[key] = value;
            }
        }

        [Obsolete("Use GetSettingsValue instead")]
        public T GetSetting<T>(string key)
        {
            return GetSettingsValue<T>(key);
        }

        public void ResetToDefaults(IModConfig defaultConfig)
        {
            Enabled = defaultConfig.Enabled;
            RequireVR = defaultConfig.RequireVR;
            Settings = new Dictionary<string, object>(defaultConfig.Settings);
        }

        public void MakeConsistentWithDefaults(IModConfig defaultConfig)
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
                }
                else if (!IsSettingSameType(setting.Value, defaultConfig.Settings[setting.Key]))
                {
                    settingsCopy[setting.Key] = defaultConfig.Settings[setting.Key];
                }
            }
            Settings = settingsCopy;
        }

        private void AddMissingDefaults(IModConfig defaultConfig)
        {
            var missingSettings = defaultConfig.Settings.Where(s => !Settings.ContainsKey(s.Key)).ToList();
            missingSettings.ForEach(setting => Settings.Add(setting.Key, setting.Value));
        }

        private bool IsSettingSameType(object settingValue1, object settingValue2)
        {
            return settingValue1.GetType() == settingValue2.GetType() &&
                   (!(settingValue1 is JObject obj1) || !(settingValue2 is JObject obj2) ||
                    (string)obj1["type"] == (string)obj2["type"]);
        }
    }
}
