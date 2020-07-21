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

            return GetSettingsValue<T>(key, Settings[key]);
        }

        private T GetSettingsValue<T>(string key, object setting)
        {
            try
            {
                var val = setting is JObject obj ? obj["value"] : setting;
                return (T)Convert.ChangeType(val, typeof(T));
            }
            catch (InvalidCastException)
            {
                ModConsole.Instance.WriteLine($"Error when converting setting {key} of type {setting.GetType()} to type {typeof(T)}");
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

            var toRemove = Settings.Keys.Except(defaultConfig.Settings.Keys).ToList();
            toRemove.ForEach(key => Settings.Remove(key));

            var keysCopy = Settings.Keys.ToList();
            foreach (var key in keysCopy)
            {
                if (!IsSettingSameType(Settings[key], defaultConfig.Settings[key]))
                {
                    TryUpdate(key, Settings[key], defaultConfig.Settings[key]);
                }
            }

            AddMissingDefaults(defaultConfig);
        }

        private void AddMissingDefaults(IModConfig defaultConfig)
        {
            var missingSettings = defaultConfig.Settings.Where(s => !Settings.ContainsKey(s.Key)).ToList();
            missingSettings.ForEach(setting => Settings.Add(setting.Key, setting.Value));
        }

        private object ToNumber(object value)
        {
            if (value is string stringValue)
            {
                return stringValue.Contains('.') || stringValue.Contains('e') ?
                    Convert.ToDouble(stringValue) : Convert.ToInt64(stringValue);
            }
            else return value;
        }

        private void TryUpdate(string key, object userSetting, object modderSetting)
        {
            var userValue = GetSettingsValue<object>(key, userSetting);
            if (userValue is JValue userJValue)
            {
                userValue = userJValue.Value;
            }
            bool isUpdateable = false;
            if (IsNumber(userSetting) && IsNumber(modderSetting))
            {
                userValue = ToNumber(userValue);
                isUpdateable = true;
            }
            if (IsBoolean(userSetting) && IsBoolean(modderSetting))
            {
                userValue = Convert.ToBoolean(userValue);
                isUpdateable = true;
            }
            Settings[key] = modderSetting;
            if (isUpdateable)
            {   
                SetSettingsValue(key, userValue);
            }
        }

        private bool IsNumber(object setting)
        {
            if (setting is JObject settingObject)
            {
                switch ((string)settingObject["type"])//selector soon to be added, would rather keep it
                {
                    case "slider":
                        return true;
                    default:
                        return false;
                }
            }
            return new[] { typeof(long), typeof(int), typeof(float), typeof(double) }.Contains(setting.GetType());
        }

        private bool IsBoolean(object setting)
        {
            if (setting is JObject settingObject)
            {
                switch ((string)settingObject["type"])//I suppose it should be replaced with simple (*?*:*)
                {
                    case "toggle":
                        return true;
                    default:
                        return false;
                }
            }
            return setting is bool;
        }

        private bool IsSettingSameType(object settingValue1, object settingValue2)
        {
            return settingValue1.GetType() == settingValue2.GetType() &&
                   (!(settingValue1 is JObject obj1) || !(settingValue2 is JObject obj2) ||
                    (string)obj1["type"] == (string)obj2["type"]);
        }
    }
}
