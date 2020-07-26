using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using OWML.Common;
using OWML.ModHelper;

namespace OWML.ModLoader
{
    public class ModData : IModData
    {
        public IModManifest Manifest { get; }
        public IModConfig Config { get; private set; }
        public IModConfig DefaultConfig { get; private set; }
        public bool RequireReload => Config.Enabled != _configSnapshot.Enabled;

        public bool Enabled => Config != null && Config.Enabled ||
                               Config == null && DefaultConfig != null && DefaultConfig.Enabled ||
                               Config == null && DefaultConfig == null;

        public bool RequireVR => Manifest.RequireVR ||
                                 Config != null && Config.RequireVR ||
                                 Config == null && DefaultConfig != null && DefaultConfig.RequireVR;

        private readonly IModConfig _configSnapshot;

        public ModData(IModManifest manifest, IModConfig config, IModConfig defaultConfig)
        {
            Manifest = manifest;
            Config = config;
            DefaultConfig = defaultConfig;
            _configSnapshot = config.Copy();
        }

        public void UpdateSnapshot()
        {
            _configSnapshot.Enabled = Enabled;
        }

        public void ResetConfigToDefaults()
        {
            Config = DefaultConfig.Copy();
        }

        public bool FixConfigs()
        {
            var settingsChanged = false;
            var storage = new ModStorage(Manifest);
            if (Config == null && DefaultConfig == null)
            {
                Config = new ModConfig();
                DefaultConfig = new ModConfig();
            }
            else if (DefaultConfig != null && Config == null)
            {
                Config = DefaultConfig;
            }
            else if (DefaultConfig != null)
            {
                settingsChanged = !MakeConfigConsistentWithDefault();
            }
            storage.Save(Config, Constants.ModConfigFileName);
            UpdateSnapshot();
            return settingsChanged;
        }

        private bool MakeConfigConsistentWithDefault()
        {
            if (DefaultConfig == null)
            {
                return true;
            }
            var wasCompatible = true;
            var toRemove = Config.Settings.Keys.Except(DefaultConfig.Settings.Keys).ToList();
            toRemove.ForEach(key => Config.Settings.Remove(key));

            var keysCopy = Config.Settings.Keys.ToList();
            foreach (var key in keysCopy)
            {
                if (!IsSettingSameType(Config.Settings[key], DefaultConfig.Settings[key]))
                {
                    wasCompatible = TryUpdate(key, Config.Settings[key], DefaultConfig.Settings[key]) && wasCompatible;
                }
                else if (DefaultConfig.Settings[key] is JObject objectValue && objectValue["type"].ToString() == "selector")
                {
                    wasCompatible = UpdateSelector(key, Config.Settings[key], objectValue) && wasCompatible;
                }
            }

            AddMissingDefaults(DefaultConfig);
            return wasCompatible;
        }

        private bool UpdateSelector(string key, object userSetting, JObject modSetting)
        {
            var options = modSetting["options"].ToObject<List<string>>();
            var userString = userSetting is JObject objectValue ? (string)objectValue["value"] : Convert.ToString(userSetting);
            Config.Settings[key] = modSetting;
            var isInOptions = options.Contains(userString);
            if (isInOptions)
            {
                Config.SetSettingsValue(key, userString);
            }
            return isInOptions;
        }

        private void AddMissingDefaults(IModConfig defaultConfig)
        {
            var missingSettings = defaultConfig.Settings.Where(s => !Config.Settings.ContainsKey(s.Key)).ToList();
            missingSettings.ForEach(setting => Config.Settings.Add(setting.Key, setting.Value));
        }

        private bool TryUpdate(string key, object userSetting, object modSetting)
        {
            var userValue = Config.GetSettingsValue<object>(key);
            if (userValue is JValue userJValue)
            {
                userValue = userJValue.Value;
            }
            Config.Settings[key] = modSetting;

            if (IsNumber(userSetting) && IsNumber(modSetting))
            {
                Config.SetSettingsValue(key, Convert.ToDouble(userValue));
                return true;
            }

            if (IsBoolean(userSetting) && IsBoolean(modSetting))
            {
                Config.SetSettingsValue(key, Convert.ToBoolean(userValue));
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
