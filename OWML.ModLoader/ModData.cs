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
        public bool Enabled => (Config != null && Config.Enabled)
                 || (Config == null && DefaultConfig != null && DefaultConfig.Enabled)
                 || (Config == null && DefaultConfig == null);
        public bool RequireVR => Manifest.RequireVR
            || (Config != null && Config.RequireVR)
            || (Config == null && DefaultConfig != null && DefaultConfig.RequireVR);

    public ModData(IModManifest manifest, IModConfig config, IModConfig defaultConfig)
        {
            Manifest = manifest;
            Config = config;
            DefaultConfig = defaultConfig;
        }

        public void ResetConfigToDefaults()
        {
            Config.Enabled = DefaultConfig.Enabled;
            Config.Settings = new Dictionary<string, object>(DefaultConfig.Settings);
        }

        public void FixConfigs()
        {
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
                MakeConfigConsistentWithDefault();
            }
            storage.Save(Config, Constants.ModConfigFileName);
        }

        private void MakeConfigConsistentWithDefault()
        {
            if (DefaultConfig == null)
            {
                return;
            }

            var toRemove = Config.Settings.Keys.Except(DefaultConfig.Settings.Keys).ToList();
            toRemove.ForEach(key => Config.Settings.Remove(key));

            var keysCopy = Config.Settings.Keys.ToList();
            foreach (var key in keysCopy)
            {
                if (!IsSettingSameType(Config.Settings[key], DefaultConfig.Settings[key]))
                {
                    TryUpdate(key, Config.Settings[key], DefaultConfig.Settings[key]);
                }
                else if (DefaultConfig.Settings[key] is JObject objectValue && objectValue["type"].ToString() == "selector")
                {
                    UpdateSelector(key, Config.Settings[key], objectValue);
                }
            }

            AddMissingDefaults(DefaultConfig);
        }

        private bool UpdateSelector(string key, object userSetting, JObject modderSetting)
        {
            var options = modderSetting["options"].ToObject<List<string>>();
            var userString = userSetting is JObject objectValue ? (string)objectValue["value"] : Convert.ToString(userSetting);
            Config.Settings[key] = modderSetting;
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

        private bool TryUpdate(string key, object userSetting, object modderSetting)
        {
            var userValue = Config.GetSettingsValue<object>(key);
            if (userValue is JValue userJValue)
            {
                userValue = userJValue.Value;
            }
            Config.Settings[key] = modderSetting;

            if (IsNumber(userSetting) && IsNumber(modderSetting))
            {
                Config.SetSettingsValue(key, Convert.ToDouble(userValue));
                return true;
            }

            if (IsBoolean(userSetting) && IsBoolean(modderSetting))
            {
                Config.SetSettingsValue(key, Convert.ToBoolean(userValue));
                return true;
            }
            return false;
        }

        private bool IsNumber(object setting)
        {
            if (setting is JObject settingObject)
            {
                return settingObject["type"].ToString() == "slider";
            }
            return new[] { typeof(long), typeof(int), typeof(float), typeof(double) }.Contains(setting.GetType());
        }

        private bool IsBoolean(object setting)
        {
            return setting is JObject settingObject ? settingObject["type"].ToString() == "toggle" : setting is bool;
        }

        private bool IsSettingSameType(object settingValue1, object settingValue2)
        {
            return settingValue1.GetType() == settingValue2.GetType() &&
                   (!(settingValue1 is JObject obj1) || !(settingValue2 is JObject obj2) ||
                    (string)obj1["type"] == (string)obj2["type"]);
        }
    }
}
