using System.Collections.Generic;
using OWML.Common;
using System.Linq;
using OWML.Common.Menus;

namespace OWML.ModHelper.Menus
{
    public class ModConfigMenu : BaseConfigMenu, IModConfigMenu
    {
        private const string EnabledTitle = "Enabled";
        private const string RequiresVRTitle = "Requires VR";

        public IModBehaviour Mod { get; }

        private readonly IModConfig _config;
        private readonly IModConfig _defaultConfig;

        public ModConfigMenu(IModConsole console, IModManifest manifest, IModConfig config, IModConfig defaultConfig, IModBehaviour mod) 
            : base(console, manifest)
        {
            Mod = mod;
            _config = config;
            _defaultConfig = defaultConfig;
        }
        
        protected override void AddInputs()
        {
            var index = 2;
            AddConfigInput(EnabledTitle, _config.Enabled, index++);
            AddConfigInput(RequiresVRTitle, _config.RequireVR, index++);
            foreach (var setting in _config.Settings)
            {
                AddConfigInput(setting.Key, setting.Value, index++);
            }
            SelectFirst();
            UpdateNavigation();
        }

        protected override void UpdateUIValues()
        {
            GetToggleInput(EnabledTitle).Value = _config.Enabled;
            GetToggleInput(RequiresVRTitle).Value = _config.RequireVR;
            foreach (var setting in _config.Settings)
            {
                SetInputValue(setting.Key, setting.Value);
            }
        }

        protected override void OnSave()
        {
            _config.Enabled = (bool)GetInputValue(EnabledTitle);
            _config.RequireVR = (bool)GetInputValue(RequiresVRTitle);
            var keys = _config.Settings.Select(x => x.Key).ToList();
            foreach (var key in keys)
            {
                var value = GetInputValue(key);
                _config.SetSettingsValue(key, value);
            }
            Storage.Save(_config, "config.json");
            Mod?.Configure(_config);
            Close();
        }

        protected override void OnReset()
        {
            _config.Enabled = _defaultConfig.Enabled;
            _config.RequireVR = _defaultConfig.RequireVR;
            _config.Settings = new Dictionary<string, object>(_defaultConfig.Settings);
            UpdateUIValues();
        }

    }
}
