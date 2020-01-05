using System.Collections.Generic;

namespace OWML.Common
{
    public interface IModConfig
    {
        Dictionary<string, object> Settings { get; }
        T GetSetting<T>(string key);
    }
}
