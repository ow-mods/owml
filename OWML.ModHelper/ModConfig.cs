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

        public T GetValue<T>(string key)
        {
            if (!Settings.ContainsKey(key))
            {
                ModConsole.Instance.WriteLine("Error: setting not found: " + key);
                return default;
            }

            var value = Settings[key];

            try
            {
                if (value is long l)
                {
                    return (T)(object)Convert.ToSingle(l);
                }

                if (value is int i)
                {
                    return (T)(object)Convert.ToSingle(i);
                }

                if (value is double d)
                {
                    return (T)(object)Convert.ToSingle(d);
                }

                if (value is JObject obj)
                {
                    var type = (string)obj["type"];
                    if (type == "toggle")
                    {
                        return (T)(object)(bool)obj["value"];
                    }
                    if (type == "slider")
                    {
                        return (T)(object)(float)obj["value"];
                    }
                }

                return (T)value;
            }
            catch (InvalidCastException)
            {
                ModConsole.Instance.WriteLine($"Error when converting a value of type {value.GetType()} to type {typeof(T)} from setting {key}");
                return default;
            }
        }

        [Obsolete("Use GetValue instead")]
        public T GetSetting<T>(string key)
        {
            return GetValue<T>(key);
        }

    }
}
