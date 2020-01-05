using System.Collections.Generic;
using Newtonsoft.Json;
using OWML.Common;

namespace OWML.ModHelper
{
    public class ModConfig : IModConfig
    {
        [JsonProperty("settings")]
        public Dictionary<string, object> Settings { get; set; }

        public ModConfig()
        {
            Settings = new Dictionary<string, object>();
        }

        public T GetSetting<T>(string key)
        {
            if (Settings.ContainsKey(key))
            {
                return (T)Settings[key];
            }
            return default;
        }

    }
}
