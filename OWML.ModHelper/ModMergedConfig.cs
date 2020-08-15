using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using OWML.Common;

namespace OWML.ModHelper
{
    public class ModMergedConfig : IModConfig
    {
        public bool Enabled { get => _userConfig.Enabled; set => _userConfig.Enabled = value; }
        public Dictionary<string, object> Settings
        {
            get {
                var settings = new Dictionary<string, object>(_defaultConfig.Settings);
                _userConfig.Settings.ToList().ForEach(x => settings[x.Key] = x.Value);
                return settings;
            }
            set => _userConfig.Settings = value;
        }

        private IModConfig _userConfig;
        private IModConfig _defaultConfig;
        private IModManifest _manifest;

        public ModMergedConfig(IModConfig userConfig, IModConfig defaultConfig, IModManifest manifest)
        {
            _userConfig = userConfig;
            _defaultConfig = defaultConfig;
            _manifest = manifest;
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

        private bool FixConfigs()
        {
            var settingsChanged = false;
            var storage = new ModStorage(_manifest);
            if (_userConfig == null)
            {
                _userConfig = _defaultConfig.Copy();
            }
            else if (_defaultConfig != null)
            {
                settingsChanged = !MakeConfigConsistentWithDefault();
            }
            storage.Save(_userConfig, Constants.ModConfigFileName);
            return settingsChanged;
        }

        private bool MakeConfigConsistentWithDefault()
        {
            var wasCompatible = true;
            var toRemove = _userConfig.Settings.Keys.Except(_defaultConfig.Settings.Keys).ToList();
            toRemove.ForEach(key => _userConfig.Settings.Remove(key));

            var keysCopy = _userConfig.Settings.Keys.ToList();
            foreach (var key in keysCopy)
            {
                if (!IsSettingSameType(_userConfig.Settings[key], _defaultConfig.Settings[key]))
                {
                    wasCompatible = TryUpdate(key, _userConfig.Settings[key], _defaultConfig.Settings[key]) && wasCompatible;
                }
                else if (_defaultConfig.Settings[key] is JObject objectValue && objectValue["type"].ToString() == "selector")
                {
                    wasCompatible = UpdateSelector(key, _userConfig.Settings[key], objectValue) && wasCompatible;
                }
            }

            AddMissingDefaults(_defaultConfig);
            return wasCompatible;
        }

        private bool UpdateSelector(string key, object userSetting, JObject modSetting)
        {
            var options = modSetting["options"].ToObject<List<string>>();
            var userString = userSetting is JObject objectValue ? (string)objectValue["value"] : Convert.ToString(userSetting);
            _userConfig.Settings[key] = modSetting;
            var isInOptions = options.Contains(userString);
            if (isInOptions)
            {
                _userConfig.SetSettingsValue(key, userString);
            }
            return isInOptions;
        }

        private void AddMissingDefaults(IModConfig defaultConfig)
        {
            var missingSettings = defaultConfig.Settings.Where(s => !_userConfig.Settings.ContainsKey(s.Key)).ToList();
            missingSettings.ForEach(setting => _userConfig.Settings.Add(setting.Key, setting.Value));
        }

        private bool TryUpdate(string key, object userSetting, object modSetting)
        {
            var userValue = _userConfig.GetSettingsValue<object>(key);
            if (userValue is JValue userJValue)
            {
                userValue = userJValue.Value;
            }
            _userConfig.Settings[key] = modSetting;

            if (IsNumber(userSetting) && IsNumber(modSetting))
            {
                _userConfig.SetSettingsValue(key, Convert.ToDouble(userValue));
                return true;
            }

            if (IsBoolean(userSetting) && IsBoolean(modSetting))
            {
                _userConfig.SetSettingsValue(key, Convert.ToBoolean(userValue));
                return true;
            }
            return false;
        }

        private bool IsNumber(object setting)
        {
            return setting is JObject settingObject
                ? settingObject["type"].ToString() == "slider"
                : new[] { typeof(long), typeof(int), typeof(float), typeof(double) }.Contains(setting.GetType());
        }

        private bool IsBoolean(object setting)
        {
            return setting is JObject settingObject
                ? settingObject["type"].ToString() == "toggle"
                : setting is bool;
        }

        private bool IsSettingSameType(object settingValue1, object settingValue2)
        {
            return settingValue1.GetType() == settingValue2.GetType() &&
                   (!(settingValue1 is JObject obj1) || !(settingValue2 is JObject obj2) ||
                    (string)obj1["type"] == (string)obj2["type"]);
        }
    }
}
