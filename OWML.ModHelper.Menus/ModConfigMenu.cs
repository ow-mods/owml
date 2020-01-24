using Newtonsoft.Json.Linq;
using OWML.Common;
using OWML.Common.Menus;
using System;
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

        private IModToggleInput _toggleTemplate;
        private IModSliderInput _sliderTemplate;

        public ModConfigMenu(IModLogger logger, IModConsole console, IModData modData, IModBehaviour mod) : base(logger, console)
        {
            _logger = logger;
            _console = console;
            ModData = modData;
            Mod = mod;
        }

        public void Initialize(Menu menu, IModToggleInput toggleTemplate, IModSliderInput sliderTemplate)
        {
            _toggleTemplate = toggleTemplate;
            _sliderTemplate = sliderTemplate;

            var layoutGroup = menu.GetComponentsInChildren<VerticalLayoutGroup>().Single(x => x.name == "Content");

            Initialize(menu, layoutGroup);

            Title = ModData.Manifest.Name;
            GetButton("UIElement-CancelOutOfRebinding").Hide();

            var index = 2;
            AddConfigInput("Enabled", ModData.Config.Enabled, index++);
            AddConfigInput("Requires VR", ModData.Config.RequireVR, index++);
            foreach (var setting in ModData.Config.Settings)
            {
                AddConfigInput(setting, index++);
            }
        }

        private void AddConfigInput(KeyValuePair<string, object> kvPair, int index)
        {
            AddConfigInput(kvPair.Key, kvPair.Value, index);
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
            toggle.YesButton.Title = (string)obj["left"];
            toggle.NoButton.Title = (string)obj["right"];
        }

        private void AddSliderInput(string key, JObject obj, int index)
        {
            var slider = AddSliderInput(_sliderTemplate.Copy(key), index);
            slider.Value = (float)obj["value"];
            slider.Min = (float)obj["min"];
            slider.Max = (float)obj["max"];
        }

        private void AddTextInput(string key, object value, int index)
        {
            _console.WriteLine("Error: AddTextInput is not implemented.");
        }

        private void AddToggleInput(string key, bool value, int index)
        {
            var toggle = AddToggleInput(_toggleTemplate.Copy(key), index);
            toggle.Value = value;
            toggle.YesButton.Title = "Yes";
            toggle.YesButton.Title = "No";
        }

    }
}
