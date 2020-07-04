using OWML.Common;

namespace OWML.ModHelper.Menus
{
    public class OwmlConfigMenu : ModConfigMenu
    {
        private readonly IOwmlConfig _config;

        public OwmlConfigMenu(IModConsole console, IModData owmlData, IOwmlConfig config) : base(console, owmlData, null)
        {
            _config = config;
        }

        protected override void AddInputs()
        {
            var index = 2;
            AddConfigInput("Game path", _config.GamePath, index++);
            AddConfigInput("Verbose mode", _config.Verbose, index++);
            AddConfigInput("Block combination input", _config.BlockInput, index++);
            SelectFirst();
            UpdateNavigation();
        }

        protected override void UpdateUIValues()
        {
            GetTextInput("Game path").Value = _config.GamePath;
            GetToggleInput("Verbose mode").Value = _config.Verbose;
            GetToggleInput("Block combination input").Value = _config.BlockInput;
        }

        protected override void OnSave()
        {
            _config.GamePath = (string)GetInputValue("Game path");
            _config.Verbose = (bool)GetInputValue("Verbose mode");
            _config.BlockInput = (bool)GetInputValue("Block combination input");
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
