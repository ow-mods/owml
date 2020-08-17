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
            get => _defaultConfig.Settings;
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

        private void FixConfigs()
        {
            MakeConfigConsistentWithDefault();
            SaveToStorage();
        }

        private void RemoveRedundantUserConfigEntries()
        {
            var toRemove = _userConfig.Settings.Keys.Except(_defaultConfig.Settings.Keys).ToList();
            toRemove.ForEach(key => _userConfig.Settings.Remove(key));
        }

        private void MakeConfigConsistentWithDefault()
        {
            RemoveRedundantUserConfigEntries();
            var keysCopy = _userConfig.Settings.Keys.ToList();
            foreach (var key in keysCopy)
            {
                var userValue = _userConfig.Settings[key];
                var defaultValue = _defaultConfig.Settings[key];
                if (!IsSettingConsistentWithDefault(userValue, defaultValue))
                {
                    _userConfig.Settings.Remove(key);
                }
            }
        }

        private object GetInnerValue(object outerValue)
        {
            if (outerValue is JObject jObject)
            {
                return jObject["value"].ToObject(typeof(object));
            }
            return outerValue;
        }

        private bool IsNumeric(object value)
        {
            var type = value.GetType();
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            return underlyingType.IsPrimitive || underlyingType == typeof(decimal);
        }

        private bool IsSettingConsistentWithDefault(object userValue, object defaultValue)
        {
            var defaultInnerValue = GetInnerValue(defaultValue);
            if (IsNumeric(userValue) && IsNumeric(defaultInnerValue))
            {
                return true;
            }
            return userValue.GetType() == defaultInnerValue.GetType();
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
