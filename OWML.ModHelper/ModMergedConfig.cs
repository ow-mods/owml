using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using OWML.Common;

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
            return _defaultConfig.GetSettingsValue<T>(key);
        }

        public void SetSettingsValue(string key, object value)
        {
            _userConfig.SetSettingsValue(key, GetConvertedSelectorValue(key, value) ?? value);
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

        private object GetInnerValue(object outerValue)
        {
            if (outerValue is JObject jObject)
            {
                return jObject["value"].ToObject(typeof(object));
            }
            return outerValue;
        }

        private void SetInnerValue(Dictionary<string, object> settings, string key, object value)
        {
            if (settings[key] is JObject jObject)
            {
                jObject["value"] = JToken.FromObject(value);
                return;
            }
            settings[key] = value;
        }

        private bool IsNumber(object value)
        {
            return value is int
                || value is long
                || value is float
                || value is double;
        }

        private bool IsSettingConsistentWithDefault(string key)
        {
            if (!_defaultConfig.Settings.ContainsKey(key))
            {
                return false;
            }
            var userValue = _userConfig.Settings[key];
            var defaultValue = _defaultConfig.Settings[key];
            var defaultInnerValue = GetInnerValue(defaultValue);
            return (userValue.GetType() == defaultInnerValue.GetType())
                || (IsNumber(userValue) && IsNumber(defaultInnerValue));
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

        private object GetConvertedSelectorValue(string key, object value)
        {
            if (!_defaultConfig.Settings.ContainsKey(key))
            {
                return null;
            }
            var defaultValue = _defaultConfig.Settings[key];
            if (defaultValue is JObject defaultObjectValue && defaultObjectValue["type"].ToString() == "selector")
            {
                var innerValue = defaultObjectValue["value"];
                return innerValue.Type == JTokenType.Integer ? int.Parse(value.ToString()) : value;
            }
            return null;
        }
    }
}
