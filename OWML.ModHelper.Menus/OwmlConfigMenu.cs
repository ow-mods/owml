using OWML.Common;

namespace OWML.ModHelper.Menus
{
    public class OwmlConfigMenu : ModConfigMenuBase
    {
        private const string VerboseModeTitle = "Verbose mode";
        private const string BlockInputTitle = "Mod input combinations block game input";

        private readonly IOwmlConfig _config;
        private readonly IOwmlConfig _defaultConfig;

        public OwmlConfigMenu(IModConsole console, IModManifest manifest, IOwmlConfig config, IOwmlConfig defaultConfig)
            : base(console, manifest)
        {
            _config = config;
            _defaultConfig = defaultConfig;
        }

        protected override void AddInputs()
        {
            var index = 2;
            AddConfigInput(VerboseModeTitle, _config.Verbose, index++);
            AddConfigInput(BlockInputTitle, _config.BlockInput, index++);
            UpdateNavigation();
            SelectFirst();
        }

        protected override void UpdateUIValues()
        {
            GetToggleInput(VerboseModeTitle).Value = _config.Verbose;
            GetToggleInput(BlockInputTitle).Value = _config.BlockInput;
        }

        protected override void OnSave()
        {
            _config.Verbose = (bool)GetInputValue(VerboseModeTitle);
            _config.BlockInput = (bool)GetInputValue(BlockInputTitle);
            Storage.Save(_config, Constants.OwmlConfigFileName);
            Close();
        }

        protected override void OnReset()
        {
            _config.Verbose = _defaultConfig.Verbose;
            _config.BlockInput = _defaultConfig.BlockInput;
            UpdateUIValues();
        }

    }
}
