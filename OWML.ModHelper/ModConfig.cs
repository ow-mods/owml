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
                ModOutput.OwmlOutput.WriteLine("Error: setting not found: " + key);
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
                ModOutput.OwmlOutput.WriteLine($"Error when converting setting {key} of type {value.GetType()} to type {typeof(T)}");
                return default;
            }
        }

        public void SetSettingsValue(string key, object val)
        {
            if (!Settings.ContainsKey(key))
            {
                ModOutput.OwmlOutput.WriteLine("Error: setting not found: " + key);
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

    }
}
