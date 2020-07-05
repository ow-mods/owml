using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Events;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModConfigMenu : ModPopupMenu, IModConfigMenu
    {
        private const string EnabledTitle = "Enabled";
        private const string RequiresVRTitle = "Requires VR";

        public IModManifest Manifest { get; }
        public IModConfig Config { get; }
        public IModConfig DefaultConfig { get; }
        public IModBehaviour Mod { get; }

        private readonly IModConsole _console;
        protected readonly IModStorage Storage;

        private IModToggleInput _toggleTemplate;
        private IModSliderInput _sliderTemplate;
        private IModTextInput _textInputTemplate;
        private IModComboInput _comboInputTemplate;
        private IModNumberInput _numberInputTemplate;

        public ModConfigMenu(IModConsole console, IModManifest manifest, IModConfig config, IModConfig defaultConfig, IModBehaviour mod) : base(console)
        {
            _console = console;
            Manifest = manifest;
            Config = config;
            DefaultConfig = defaultConfig;
            Mod = mod;
            Storage = new ModStorage(console, manifest);
        }

        public void Initialize(Menu menu, IModToggleInput toggleTemplate, IModSliderInput sliderTemplate,
            IModTextInput textInputTemplate, IModNumberInput numberInputTemplate, IModComboInput comboInputTemplate)
        {
            _toggleTemplate = toggleTemplate;
            _sliderTemplate = sliderTemplate;
            _textInputTemplate = textInputTemplate;
            _numberInputTemplate = numberInputTemplate;
            _comboInputTemplate = comboInputTemplate;

            var layoutGroup = menu.GetComponentsInChildren<VerticalLayoutGroup>().Single(x => x.name == "Content");
            Initialize(menu, layoutGroup);

            var blocker = menu.GetComponentsInChildren<GraphicRaycaster>().Single(x => x.name == "RebindingModeBlocker");
            blocker.gameObject.SetActive(false);

            var labelPanel = menu.GetValue<GameObject>("_selectableItemsRoot").GetComponentInChildren<HorizontalLayoutGroup>();
            labelPanel.gameObject.SetActive(false);

            Title = Manifest.Name;

            var saveButton = GetButton("UIElement-SaveAndExit");
            var resetButton = GetButton("UIElement-ResetToDefaultsButton");
            var cancelButton = GetButton("UIElement-DiscardChangesButton");

            saveButton.OnClick += OnSave;
            resetButton.OnClick += OnReset;
            cancelButton.OnClick += Close;

            saveButton.SetControllerCommand(InputLibrary.confirm);
            resetButton.SetControllerCommand(InputLibrary.setDefaults);
            cancelButton.SetControllerCommand(InputLibrary.cancel);

            GetButton("UIElement-CancelOutOfRebinding").Hide();
            GetButton("UIElement-KeyRebinder").Hide();

            foreach (Transform child in layoutGroup.transform)
            {
                child.gameObject.SetActive(false);
            }

            AddInputs();
        }

        public override void Open()
        {
            base.Open();
            UpdateUIValues();
        }

        protected virtual void AddInputs()
        {
            var index = 2;
            AddConfigInput(EnabledTitle, Config.Enabled, index++);
            AddConfigInput(RequiresVRTitle, Config.RequireVR, index++);
            foreach (var setting in Config.Settings)
            {
                AddConfigInput(setting.Key, setting.Value, index++);
            }
            SelectFirst();
            UpdateNavigation();
        }

        protected virtual void UpdateUIValues()
        {
            GetToggleInput(EnabledTitle).Value = Config.Enabled;
            GetToggleInput(RequiresVRTitle).Value = Config.RequireVR;
            foreach (var setting in Config.Settings)
            {
                SetInputValue(setting.Key, setting.Value);
            }
        }

        protected void AddConfigInput(string key, object value, int index)
        {
            if (value is bool)
            {
                AddToggleInput(key, index);
                return;
            }

            if (value is string)
            {
                AddTextInput(key, index);
                return;
            }

            if (new[] { typeof(long), typeof(int), typeof(float), typeof(double) }.Contains(value.GetType()))
            {
                AddNumberInput(key, index);
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
                if (type == "input")
                {
                    AddComboInput(key, index);
                    return;
                }

                _console.WriteLine("Error: unrecognized complex setting: " + value);
                return;
            }

            _console.WriteLine("Error: unrecognized setting type: " + value.GetType());
        }

        private void AddToggleInput(string key, int index)
        {
            var toggle = AddToggleInput(_toggleTemplate.Copy(key), index);
            toggle.YesButton.Title = "Yes";
            toggle.NoButton.Title = "No";
            toggle.Element.name = key;
            toggle.Title = key;
            toggle.Show();
        }

        private void AddToggleInput(string key, JObject obj, int index)
        {
            var toggle = AddToggleInput(_toggleTemplate.Copy(key), index);
            toggle.YesButton.Title = (string)obj["yes"];
            toggle.NoButton.Title = (string)obj["no"];
            toggle.Element.name = key;
            toggle.Title = (string)obj["title"] ?? key;
            toggle.Show();
        }

        private void AddSliderInput(string key, JObject obj, int index)
        {
            var slider = AddSliderInput(_sliderTemplate.Copy(key), index);
            slider.Min = (float)obj["min"];
            slider.Max = (float)obj["max"];
            slider.Element.name = key;
            slider.Title = (string)obj["title"] ?? key;
            slider.Show();
        }

        private void AddTextInput(string key, int index)
        {
            var textInput = AddTextInput(_textInputTemplate.Copy(key), index);
            textInput.Element.name = key;
            textInput.Show();
        }

        private void AddComboInput(string key, int index)
        {
            var comboInput = AddComboInput(_comboInputTemplate.Copy(key), index);
            comboInput.Element.name = key;
            comboInput.Show();
        }

        private void AddNumberInput(string key, int index)
        {
            var numberInput = AddNumberInput(_numberInputTemplate.Copy(key), index);
            numberInput.Element.name = key;
            numberInput.Show();
        }

        protected virtual void OnSave()
        {
            Config.Enabled = (bool)GetInputValue(EnabledTitle);
            Config.RequireVR = (bool)GetInputValue(RequiresVRTitle);
            var keys = Config.Settings.Select(x => x.Key).ToList();
            foreach (var key in keys)
            {
                var value = GetInputValue(key);
                Config.SetSettingsValue(key, value);
            }
            Storage.Save(Config, "config.json");
            Mod?.Configure(Config);
            Close();
        }

        protected virtual void OnReset()
        {
            Config.Enabled = DefaultConfig.Enabled;
            Config.RequireVR = DefaultConfig.RequireVR;
            Config.Settings = new Dictionary<string, object>(DefaultConfig.Settings);
            UpdateUIValues();
        }

    }
}
