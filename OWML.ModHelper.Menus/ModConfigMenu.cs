using Newtonsoft.Json.Linq;
using OWML.Common;
using OWML.Common.Menus;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModConfigMenu : ModPopupMenu, IModConfigMenu
    {
        public IModData ModData { get; }
        public IModBehaviour Mod { get; }

        private IModLogger _logger;
        private IModConsole _console;
        private IModStorage _storage;

        private IModToggleInput _toggleTemplate;
        private IModSliderInput _sliderTemplate;
        private IModTextInput _textInputTemplate;

        public ModConfigMenu(IModLogger logger, IModConsole console, IModData modData, IModBehaviour mod) : base(logger, console)
        {
            _logger = logger;
            _console = console;
            ModData = modData;
            Mod = mod;
            _storage = new ModStorage(console, modData.Manifest);
        }

        public void Initialize(Menu menu, IModToggleInput toggleTemplate, IModSliderInput sliderTemplate, IModTextInput textInputTemplate)
        {
            _toggleTemplate = toggleTemplate;
            _sliderTemplate = sliderTemplate;
            _textInputTemplate = textInputTemplate;

            var layoutGroup = menu.GetComponentsInChildren<VerticalLayoutGroup>().Single(x => x.name == "Content");
            Initialize(menu, layoutGroup);

            var blocker = menu.GetComponentsInChildren<GraphicRaycaster>().Single(x => x.name == "RebindingModeBlocker");
            blocker.gameObject.SetActive(false);

            Title = ModData.Manifest.Name;
            GetButton("UIElement-CancelOutOfRebinding").Hide();

            var index = 2;
            AddConfigInput("Enabled", ModData.Config.Enabled, index++);
            AddConfigInput("Requires VR", ModData.Config.RequireVR, index++);
            foreach (var setting in ModData.Config.Settings)
            {
                AddConfigInput(setting.Key, setting.Value, index++);
            }

            GetButton("UIElement-SaveAndExit").OnClick += OnSave;
        }

        private void AddConfigInput(string key, object value, int index)
        {
            if (value is bool val)
            {
                AddToggleInput(key, val, index);
                return;
            }

            if (new[] { typeof(long), typeof(int), typeof(float), typeof(double) }.Contains(value.GetType()))
            {
                AddTextInput(key, value, index);
                return;
            }

            if (value is JObject obj)
            {
                var type = (string)obj["type"];
                if (type == "slider")
                {
                    AddSliderInput(key, obj, index);
                    return;
                }
                if (type == "toggle")
                {
                    AddToggleInput(key, obj, index);
                    return;
                }

                _console.WriteLine("Error: unrecognized complex setting: " + value);
                return;
            }

            _console.WriteLine("Error: unrecognized setting type: " + value.GetType());
        }

        private void AddToggleInput(string key, JObject obj, int index)
        {
            var toggle = AddToggleInput(_toggleTemplate.Copy(key), index);
            toggle.Value = (bool)obj["value"];
            toggle.YesButton.Title = (string)obj["yes"];
            toggle.NoButton.Title = (string)obj["no"];
            toggle.Element.name = key;
            toggle.Title = (string)obj["title"] ?? key;
        }

        private void AddSliderInput(string key, JObject obj, int index)
        {
            var slider = AddSliderInput(_sliderTemplate.Copy(key), index);
            slider.Value = (float)obj["value"];
            slider.Min = (float)obj["min"];
            slider.Max = (float)obj["max"];
            slider.Element.name = key;
            slider.Title = (string)obj["title"] ?? key;
        }

        private void AddTextInput(string key, object value, int index)
        {
            var textInput = AddTextInput(_textInputTemplate.Copy(key), index);
            textInput.Value = value.ToString();
        }

        private void AddToggleInput(string key, bool value, int index)
        {
            var toggle = AddToggleInput(_toggleTemplate.Copy(key), index);
            toggle.Value = value;
            toggle.YesButton.Title = "Yes";
            toggle.NoButton.Title = "No";
            toggle.Element.name = key;
            toggle.Title = key;
        }

        private void OnSave()
        {
            ModData.Config.Enabled = (bool)GetInputValue("Enabled");
            ModData.Config.RequireVR = (bool)GetInputValue("Requires VR");
            var settings = new Dictionary<string, object>();
            foreach (var key in ModData.Config.Settings.Keys)
            {
                var value = GetInputValue(key);
                if (value != null)
                {
                    settings[key] = value;
                }
            }
            ModData.Config.Settings = settings;
            _storage.Save(ModData.Config, "config.json");
            Mod.Configure(ModData.Config);
        }

        private object GetInputValue(string key)
        {
            var slider = GetSliderInput(key);
            if (slider != null)
            {
                return slider.Value;
            }
            var toggle = GetToggleInput(key);
            if (toggle != null)
            {
                return toggle.Value;
            }
            _console.WriteLine("Error: no input found with name " + key);
            return null;
        }

    }
}
