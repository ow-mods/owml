using Newtonsoft.Json.Linq;
using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Events;
using System;
using System.Linq;
using UnityEngine;
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
        private IModNumberInput _numberInputTemplate;

        private bool _isInputsAdded;

        public ModConfigMenu(IModLogger logger, IModConsole console, IModData modData, IModBehaviour mod) : base(logger, console)
        {
            _logger = logger;
            _console = console;
            ModData = modData;
            Mod = mod;
            _storage = new ModStorage(console, modData.Manifest);
        }

        public void Initialize(Menu menu, IModToggleInput toggleTemplate, IModSliderInput sliderTemplate, IModTextInput textInputTemplate, IModNumberInput numberInputTemplate)
        {
            _toggleTemplate = toggleTemplate;
            _sliderTemplate = sliderTemplate;
            _textInputTemplate = textInputTemplate;
            _numberInputTemplate = numberInputTemplate;

            var layoutGroup = menu.GetComponentsInChildren<VerticalLayoutGroup>().Single(x => x.name == "Content");
            Initialize(menu, layoutGroup);

            var blocker = menu.GetComponentsInChildren<GraphicRaycaster>().Single(x => x.name == "RebindingModeBlocker");
            blocker.gameObject.SetActive(false);

            var labelPanel = menu.GetValue<GameObject>("_selectableItemsRoot").GetComponentInChildren<HorizontalLayoutGroup>();
            labelPanel.gameObject.SetActive(false);

            Title = ModData.Manifest.Name;

            GetButton("UIElement-SaveAndExit").OnClick += OnSave;
            GetButton("UIElement-ResetToDefaultsButton").OnClick += OnReset;

            GetButton("UIElement-CancelOutOfRebinding").Hide();
            GetButton("UIElement-KeyRebinder").Hide();

            _isInputsAdded = false;
        }

        public override void Open()
        {
            base.Open();
            if (!_isInputsAdded)
            {
                AddInputs();
                _isInputsAdded = true;
            }
            else
            {
                UpdateUIValues();
            }
        }

        private void AddInputs()
        {
            var index = 2;
            AddConfigInput("Enabled", ModData.Config.Enabled, index++);
            AddConfigInput("Requires VR", ModData.Config.RequireVR, index++);
            foreach (var setting in ModData.Config.Settings)
            {
                AddConfigInput(setting.Key, setting.Value, index++);
            }
        }

        private void UpdateUIValues()
        {
            GetToggleInput("Enabled").Value = ModData.Config.Enabled;
            GetToggleInput("Requires VR").Value = ModData.Config.RequireVR;
            foreach (var setting in ModData.Config.Settings)
            {
                SetInputValue(setting.Key, setting.Value);
            }
        }

        private void AddConfigInput(string key, object value, int index)
        {
            if (value is bool val)
            {
                AddToggleInput(key, val, index);
                return;
            }

            if (value is string s)
            {
                AddTextInput(key, s, index);
                return;
            }

            if (new[] { typeof(long), typeof(int), typeof(float), typeof(double) }.Contains(value.GetType()))
            {
                var f = Convert.ToSingle(value);
                AddNumberInput(key, f, index);
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

        private void AddTextInput(string key, string value, int index)
        {
            var textInput = AddTextInput(_textInputTemplate.Copy(key), index);
            textInput.Value = value;
            textInput.Element.name = key;
        }

        private void AddNumberInput(string key, float value, int index)
        {
            var numberInput = AddNumberInput(_numberInputTemplate.Copy(key), index);
            numberInput.Value = value;
            numberInput.Element.name = key;
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
            var keys = ModData.Config.Settings.Select(x => x.Key).ToList();
            foreach (var key in keys)
            {
                var value = GetInputValue(key);
                ModData.Config.SetSettingsValue(key, value);
            }
            _storage.Save(ModData.Config, "config.json");
            if (Mod != null)
            {
                Mod.Configure(ModData.Config);
            }
        }

        private void OnReset()
        {
            ModData.ResetConfig();
            UpdateUIValues();
        }

    }
}
