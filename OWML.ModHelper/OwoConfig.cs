using System.Collections.Generic;
using Newtonsoft.Json;
using OWML.Common;

namespace OWML.ModHelper
{
    public class OwoConfig : IOwoConfig
    {
        [JsonProperty("settings")]
        public Dictionary<string, object> Settings { get; set; }

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
