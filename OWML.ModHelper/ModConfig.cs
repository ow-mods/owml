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
            var type = typeof(T);
            
            try
            {
                var value = setting is JObject objectValue ? objectValue["value"] : setting;
                return type.IsEnum ? ConvertToEnum<T>(value) : (T)Convert.ChangeType(value, type);
            }
            catch (InvalidCastException)
            {
                ModConsole.Instance.WriteLine($"Error when converting setting {key} of type {setting.GetType()} to type {type}");
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
                ModConsole.Instance.WriteLine($"Error: Can't convert {valueString} to enum {typeof(T)}: {ex.Message}");
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

        private bool TryUpdate(string key, object userSetting, object modderSetting)
        {
            var userValue = GetSettingsValue<object>(key, userSetting);
            if (userValue is JValue userJValue)
            {
                userValue = userJValue.Value;
            }
            bool isUpdateable = false;
            if (IsNumber(userSetting) && IsNumber(modderSetting))
            {
                userValue = Convert.ToDouble(userValue);
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
            return isUpdateable;
        }

        private bool IsNumber(object setting)
        {
            if (setting is JObject settingObject)
            {
                return settingObject["type"].ToString() == "slider";
            }
            return new[] { typeof(long), typeof(int), typeof(float), typeof(double) }.Contains(setting.GetType());
        }

        private bool IsBoolean(object setting)
        {
            return setting is JObject settingObject ? settingObject["type"].ToString() == "toggle" : setting is bool;
        }

        private bool IsSettingSameType(object settingValue1, object settingValue2)
        {
            return settingValue1.GetType() == settingValue2.GetType() &&
                   (!(settingValue1 is JObject obj1) || !(settingValue2 is JObject obj2) ||
                    (string)obj1["type"] == (string)obj2["type"]);
        }
    }
}
