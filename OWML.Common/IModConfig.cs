using System;
using System.Collections.Generic;

namespace OWML.Common
{
    public interface IModConfig
    {
        bool Enabled { get; }
        bool RequireVR { get; }
        Dictionary<string, object> Settings { get; }
        T GetValue<T>(string key);

        [Obsolete("Use GetValue instead")]
        T GetSetting<T>(string key);
    }
}
