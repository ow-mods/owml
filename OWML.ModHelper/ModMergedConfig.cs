using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using OWML.Common;
using OWML.Logging;

namespace OWML.ModHelper
{
    public class ModMergedConfig : IModMergedConfig
    {
        public bool Enabled { get => _userConfig.Enabled; set => _userConfig.Enabled = value; }
        public Dictionary<string, object> Settings
        {
            get => GetMergedSettings();
            set => _userConfig.Settings = value;
        }

        private readonly IModConfig _userConfig;
        private readonly IModConfig _defaultConfig;
        private readonly IModStorage _storage;

        public ModMergedConfig(IModConfig userConfig, IModConfig defaultConfig, IModManifest manifest)
        {
            _userConfig = userConfig ?? new ModConfig();
            _defaultConfig = defaultConfig ?? new ModConfig();
            _storage = new ModStorage(manifest);
            FixConfigs();
        }

        public T GetSettingsValue<T>(string key)
        {
            if (_userConfig.Settings.ContainsKey(key))
            {
                return _userConfig.GetSettingsValue<T>(key);
            }
            if (!_defaultConfig.Settings.ContainsKey(key))
            {
                ModConsole.OwmlConsole.WriteLine($"Error - Setting not found: {key}", MessageType.Error);
            }
            return _defaultConfig.GetSettingsValue<T>(key);
        }

        public object GetSettingsValue(string key)
        {
            return GetSettingsValue<object>(key);
        }

        public void SetSettingsValue(string key, object value)
        {
            if (IsSettingValueEqual(key, value))
            {
                _userConfig.Settings.Remove(key);
                return;
            }
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

        public void SaveToStorage()
        {
            _storage.Save(_userConfig, Constants.ModConfigFileName);
        }

        private Dictionary<string, object> GetMergedSettings()
        {
            var settings = new Dictionary<string, object>(_defaultConfig.Settings);
            _userConfig.Settings.ToList().ForEach(x => SetInnerValue(settings, x.Key, x.Value));
            return settings;
        }

        private void SetInnerValue(Dictionary<string, object> settings, string key, object value)
        {
            if (!settings.ContainsKey(key))
            {
                return;
            }
            if (settings[key] is JObject jObject)
            {
                jObject["value"] = JToken.FromObject(value);
                return;
            }
            settings[key] = value;
        }

        private bool IsSettingValueEqual(string key, object value)
        {
            var defaultValue = _defaultConfig.GetSettingsValue(key);

            if (IsNumber(value) && IsNumber(defaultValue))
            {
                return Convert.ToDouble(value) == Convert.ToDouble(defaultValue);
            }
            return Equals(defaultValue, value);
        }

        private bool IsNumber(object value)
        {
            return value is int
                || value is long
                || value is float
                || value is double
                || value is decimal
                || value is ulong;
        }

        private bool IsSettingConsistentWithDefault(string key)
        {
            var userValue = _userConfig.Settings[key];
            var defaultValue = _defaultConfig.GetSettingsValue(key);

            if (userValue == null || defaultValue == null)
            {
                return false;
            }

            return (userValue.GetType() == defaultValue.GetType())
                || (IsNumber(userValue) && IsNumber(defaultValue));
        }

        private void MakeConfigConsistentWithDefault()
        {
            _userConfig.Settings.Keys.ToList().ForEach(key =>
            {
                if (!IsSettingConsistentWithDefault(key))
                {
                    _userConfig.Settings.Remove(key);
                }
            });
        }

        private void FixConfigs()
        {
            MakeConfigConsistentWithDefault();
            SaveToStorage();
        }
    }
}
