using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OWML.Common;
using OWML.Logging;

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
                return default;
            }

            return GetSettingsValue<T>(key, Settings[key]);
        }

        public object GetSettingsValue(string key)
        {
            return GetSettingsValue<object>(key);
        }

        private T GetSettingsValue<T>(string key, object setting)
        {
            var type = typeof(T);

            try
            {
                return GetInnerValue<T>(setting);
            }
            catch (InvalidCastException)
            {
                ModConsole.OwmlConsole.WriteLine($"Error when converting setting {key} of type {setting.GetType()} to type {type}", MessageType.Error);
                return default;
            }
        }

        private T GetInnerValue<T>(object outerValue)
        {
            if (outerValue is JObject jObject)
            {
                return jObject["value"].ToObject<T>();
            }
            return (T)Convert.ChangeType(outerValue, typeof(T));
        }

        public void SetSettingsValue(string key, object value)
        {
            Settings[key] = value;
        }
    }
}
