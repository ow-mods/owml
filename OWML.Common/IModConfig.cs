using System;
using System.Collections.Generic;

namespace OWML.Common
{
    public interface IModConfig
    {
        bool Enabled { get; set; }
        Dictionary<string, object> Settings { get; set; }

        T GetSettingsValue<T>(string key);
        void SetSettingsValue(string key, object value);

        [Obsolete("Use GetSettingsValue instead")]
        T GetSetting<T>(string key);

        IModConfig Copy();
    }
}
