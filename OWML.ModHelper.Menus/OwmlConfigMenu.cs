using OWML.Common;

namespace OWML.ModHelper.Menus
{
    public class OwmlConfigMenu : ModConfigMenu
    {
        private const string GamePathTitle = "Game path";
        private const string VerboseModeTitle = "Verbose mode";
        private const string BlockInputTitle = "Block combination input";

        private readonly IOwmlConfig _config;
        private readonly IOwmlConfig _defaultConfig;

        public OwmlConfigMenu(IModConsole console, IModManifest manifest, IOwmlConfig config, IOwmlConfig defaultConfig) :
            base(console, manifest, null, null, null)
        {
            _config = config;
            _defaultConfig = defaultConfig;
        }

        protected override void AddInputs()
        {
            var index = 2;
            AddConfigInput(GamePathTitle, _config.GamePath, index++);
            AddConfigInput(VerboseModeTitle, _config.Verbose, index++);
            AddConfigInput(BlockInputTitle, _config.BlockInput, index++);
            SelectFirst();
            UpdateNavigation();
        }

        protected override void UpdateUIValues()
        {
            GetTextInput(GamePathTitle).Value = _config.GamePath;
            GetToggleInput(VerboseModeTitle).Value = _config.Verbose;
            GetToggleInput(BlockInputTitle).Value = _config.BlockInput;
        }

        protected override void OnSave()
        {
            _config.GamePath = (string)GetInputValue(GamePathTitle);
            _config.Verbose = (bool)GetInputValue(VerboseModeTitle);
            _config.BlockInput = (bool)GetInputValue(BlockInputTitle);
            Storage.Save(_config, Constants.OwmlConfigFileName);
            Close();
        }

        protected override void OnReset()
        {
            _config.GamePath = _defaultConfig.GamePath;
            _config.Verbose = _defaultConfig.Verbose;
            _config.BlockInput = _defaultConfig.BlockInput;
            UpdateUIValues();
        }

    }
}
