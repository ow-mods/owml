using OWML.Common;

namespace OWML.ModHelper.Menus
{
    public class OwmlConfigMenu : ModConfigMenu
    {
        private const string GamePathTitle = "Game path";
        private const string VerboseModeTitle = "Verbose mode";
        private const string BlockInputTitle = "Block combination input";

        private readonly IOwmlConfig _config;

        public OwmlConfigMenu(IModConsole console, IModData owmlData, IOwmlConfig config) : base(console, owmlData, null)
        {
            _config = config;
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
            Storage.Save(_config, "OWML.Config.json");
            Close();
        }

        protected override void OnReset()
        {
            _config.GamePath = "C:/Program Files/Epic Games/OuterWilds";
            _config.Verbose = false;
            _config.BlockInput = false;
            UpdateUIValues();
        }

    }
}
