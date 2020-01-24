using System;
using System.Collections.Generic;

namespace OWML.Common
{
    public interface IModConfig
    {
        bool Enabled { get; set; }
        bool RequireVR { get; set; }
        Dictionary<string, object> Settings { get; set; }
        T GetValue<T>(string key);

        [Obsolete("Use GetValue instead")]
        T GetSetting<T>(string key);
    }
}
