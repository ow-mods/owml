using System.Collections.Generic;

namespace OWML.Common
{
    public interface IModConfig
    {
        bool RequireVR { get; }
        Dictionary<string, object> Settings { get; }
        T GetSetting<T>(string key);
    }
}
