using System.Collections.Generic;
using OWML.Common;
using System.Linq;
using OWML.Common.Menus;

namespace OWML.ModHelper.Menus
{
    public class ModConfigMenu : ModConfigMenuBase, IModConfigMenu
    {
        private const string EnabledTitle = "Enabled";
        private const string RequiresVRTitle = "Requires VR";

        public IModData ModData { get; }
        public IModBehaviour Mod { get; }

        public ModConfigMenu(IModConsole console, IModData modData, IModBehaviour mod) 
            : base(console, modData.Manifest)
        {
            ModData = modData;
            Mod = mod;
        }

        protected override void AddInputs()
        {
            var index = 2;
            AddConfigInput(EnabledTitle, ModData.Config.Enabled, index++);
            AddConfigInput(RequiresVRTitle, ModData.Config.RequireVR, index++);
            foreach (var setting in ModData.Config.Settings)
            {
                AddConfigInput(setting.Key, setting.Value, index++);
            }
            UpdateNavigation();
            SelectFirst();
        }

        protected override void UpdateUIValues()
        {
            GetToggleInput(EnabledTitle).Value = ModData.Config.Enabled;
            GetToggleInput(RequiresVRTitle).Value = ModData.Config.RequireVR;
            foreach (var setting in ModData.Config.Settings)
            {
                SetInputValue(setting.Key, setting.Value);
            }
        }

        protected override void OnSave()
        {
            ModData.Config.Enabled = (bool)GetInputValue(EnabledTitle);
            ModData.Config.RequireVR = (bool)GetInputValue(RequiresVRTitle);
            var keys = ModData.Config.Settings.Select(x => x.Key).ToList();
            foreach (var key in keys)
            {
                var value = GetInputValue(key);
                ModData.Config.SetSettingsValue(key, value);
            }
            Storage.Save(ModData.Config, Constants.ModConfigFileName);
            Mod?.Configure(ModData.Config);
            Close();
        }

        protected override void OnReset()
        {
            ModData.Config.Enabled = ModData.DefaultConfig.Enabled;
            ModData.Config.RequireVR = ModData.DefaultConfig.RequireVR;
            ModData.Config.Settings = new Dictionary<string, object>(ModData.DefaultConfig.Settings);
            UpdateUIValues();
        }
    }
}
