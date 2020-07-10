using Newtonsoft.Json.Linq;
using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Events;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public abstract class BaseConfigMenu : ModPopupMenu, IBaseConfigMenu
    {
        public IModManifest Manifest { get; }

        protected readonly IModStorage Storage;

        private IModToggleInput _toggleTemplate;
        private IModSliderInput _sliderTemplate;
        private IModTextInput _textInputTemplate;
        private IModComboInput _comboInputTemplate;
        private IModNumberInput _numberInputTemplate;

        protected abstract void AddInputs();
        protected abstract void UpdateUIValues();
        protected abstract void OnSave();
        protected abstract void OnReset();

        protected BaseConfigMenu(IModConsole console, IModManifest manifest) : base(console)
        {
            Manifest = manifest;
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

                OwmlConsole.WriteLine("Error: unrecognized complex setting: " + value);
                return;
            }

            OwmlConsole.WriteLine("Error: unrecognized setting type: " + value.GetType());
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

    }
}
