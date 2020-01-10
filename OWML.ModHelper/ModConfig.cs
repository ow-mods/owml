using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using OWML.Common;

namespace OWML.ModHelper
{
    public class ModConfig : IModConfig
    {
        [JsonProperty("requireVR")]
        public bool RequireVR { get; set; }

        [JsonProperty("settings")]
        public Dictionary<string, object> Settings { get; set; }

        public ModConfig()
        {
            Settings = new Dictionary<string, object>();
        }

        public T GetSetting<T>(string key)
        {
            if (!Settings.ContainsKey(key))
            {
                ModConsole.Instance.WriteLine("Error: setting not found: " + key);
                return default;
            }
            var setting = Settings[key];
            try
            {
                return (T)setting;
            }
            catch (InvalidCastException)
            {
                ModConsole.Instance.WriteLine($"Error when getting setting '{key}': can't cast {setting.GetType()} to {typeof(T)}");
                return default;
            }
        }

    }
}
