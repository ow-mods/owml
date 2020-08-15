using System.Collections.Generic;
using System.Linq;
using OWML.Common;

namespace OWML.ModHelper
{
    class ModMainConfig : IModConfig
    {
        public bool Enabled { get => _userConfig.Enabled; set => _userConfig.Enabled = value; }
        public Dictionary<string, object> Settings
        {
            get {
                var settings = new Dictionary<string, object>(_userConfig.Settings);
                _defaultConfig.Settings.ToList().ForEach(x => settings[x.Key] = x.Value);
                return settings;
            }
            set => _userConfig.Settings = value;
        }

        private readonly IModConfig _userConfig;
        private readonly IModConfig _defaultConfig;

        public ModMainConfig(IModConfig userConfig, IModConfig defaultConfig)
        {
            _userConfig = userConfig;
            _defaultConfig = defaultConfig;
        }

        public T GetSettingsValue<T>(string key)
        {
            return _userConfig.GetSettingsValue<T>(key) ?? _defaultConfig.GetSettingsValue<T>(key);
        }

        public void SetSettingsValue(string key, object value)
        {
            _userConfig.SetSettingsValue(key, value);
        }

        public IModConfig Copy()
        {
            return new ModConfig
            {
                Enabled = Enabled,
                Settings = new Dictionary<string, object>(Settings)
            };
        }
    }
}
