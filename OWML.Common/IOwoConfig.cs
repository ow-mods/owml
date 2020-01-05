using System.Collections.Generic;

namespace OWML.Common
{
    public interface IOwoConfig
    {
        Dictionary<string, object> Settings { get; }
        T GetSetting<T>(string key);
    }
}
