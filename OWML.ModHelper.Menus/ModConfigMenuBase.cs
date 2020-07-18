using Newtonsoft.Json.Linq;
using OWML.Common;
using OWML.Common.Menus;
using System.Linq;

namespace OWML.ModHelper.Menus
{
    public abstract class ModConfigMenuBase : ModMenuWithSelectables, IModConfigMenuBase
    {
        public IModManifest Manifest { get; }

        protected readonly IModStorage Storage;

        private IModToggleInput _toggleTemplate;
        private IModSliderInput _sliderTemplate;
        private IModSelectorInput _selectorTemplate;
        private IModTextInput _textInputTemplate;
        private IModComboInput _comboInputTemplate;
        private IModNumberInput _numberInputTemplate;

        protected abstract void AddInputs();
        protected abstract void UpdateUIValues();

        protected ModConfigMenuBase(IModConsole console, IModManifest manifest) : base(console)
        {
            Manifest = manifest;
            Storage = new ModStorage(manifest);
        }

        public void Initialize(Menu menu, IModToggleInput toggleTemplate, IModSliderInput sliderTemplate,
            IModTextInput textInputTemplate, IModNumberInput numberInputTemplate,
            IModComboInput comboInputTemplate, IModSelectorInput selectorTemplate)
        {
            _toggleTemplate = toggleTemplate;
            _sliderTemplate = sliderTemplate;
            _textInputTemplate = textInputTemplate;
            _numberInputTemplate = numberInputTemplate;
            _comboInputTemplate = comboInputTemplate;
            _selectorTemplate = selectorTemplate;

            base.Initialize(menu);

            Title = Manifest.Name;

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
                if (type == "selector")
                {
                    AddSelectorInput(key, obj, index);
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

        private void AddSelectorInput(string key, JObject obj, int index)
        {
            var options = ((JArray)obj["options"]).ToObject<string[]>();
            var selector = AddSelectorInput(_selectorTemplate.Copy(key), index);
            selector.Element.name = key;
            selector.Title = (string)obj["title"] ?? key;
            selector.Initialize((int)obj["value"], options);
            selector.Show();
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
