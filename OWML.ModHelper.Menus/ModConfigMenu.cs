using System.Collections.Generic;
using OWML.Common;
using System.Linq;
using OWML.Common.Menus;

namespace OWML.ModHelper.Menus
{
    public class ModConfigMenu : ModConfigMenuBase, IModConfigMenu
    {
        private const string EnabledTitle = "Enabled";

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
            foreach (var setting in ModData.Config.Settings)
            {
                SetInputValue(setting.Key, setting.Value);
            }
        }

        protected override void OnSave()
        {
            ModData.Config.Enabled = (bool)GetInputValue(EnabledTitle);
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
            ModData.Config.ResetToDefaults(ModData.DefaultConfig);
            UpdateUIValues();
        }
    }
}
