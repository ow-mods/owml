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

        [JsonProperty("settings")]
        public Dictionary<string, object> Settings { get; set; } = new Dictionary<string, object>();

        public T GetSettingsValue<T>(string key)
        {
            if (!Settings.ContainsKey(key))
            {
                ModConsole.Instance.WriteLine($"Error - Setting not found: {key}", MessageType.Error);
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
                ModConsole.Instance.WriteLine($"Error when converting setting {key} of type {setting.GetType()} to type {type}", MessageType.Error);
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
                ModConsole.Instance.WriteLine($"Error - Can't convert {valueString} to enum {typeof(T)}: {ex.Message}", MessageType.Error);
                return default;
            }
        }

        public void SetSettingsValue(string key, object value)
        {
            if (!Settings.ContainsKey(key))
            {
                ModConsole.Instance.WriteLine("Error - Setting not found: " + key, MessageType.Error);
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

        public IModConfig Copy()
        {
            return new ModConfig
            {
                Enabled = Enabled,
                Settings = new Dictionary<string, object>(Settings)
            };
        }

    }
}
