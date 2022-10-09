using Newtonsoft.Json.Linq;
using System.Linq;
using OWML.Common;
using OWML.Common.Menus;
using OWML.Utils;

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
		private IModNumberInput _numberInputTemplate;
		private IModSeparator _seperatorTemplate;

		protected abstract void AddInputs();

		public abstract void UpdateUIValues();

		protected ModConfigMenuBase(IModManifest manifest, IModStorage storage, IModConsole console)
			: base(console)
		{
			Manifest = manifest;
			Storage = storage;
		}

		public void Initialize(Menu menu, IModToggleInput toggleTemplate, IModSliderInput sliderTemplate,
			IModTextInput textInputTemplate, IModNumberInput numberInputTemplate,
			IModSelectorInput selectorTemplate, IModSeparator seperatorTemplate)
		{
			_toggleTemplate = toggleTemplate;
			_sliderTemplate = sliderTemplate;
			_textInputTemplate = textInputTemplate;
			_numberInputTemplate = numberInputTemplate;
			_selectorTemplate = selectorTemplate;
			_seperatorTemplate = seperatorTemplate;

			base.Initialize(menu);
			menu.SetValue("_menuOptions", new MenuOption[] { });

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
				var settingType = (string)obj["type"];
				switch (settingType)
				{
					case "separator":
						AddSeparator(key, index, obj);
						return;
					case "slider":
						AddSliderInput(key, index, obj);
						return;
					case "toggle":
						AddToggleInput(key, index, obj);
						return;
					case "selector":
						AddSelectorInput(key, index, obj);
						return;
					case "text":
						AddTextInput(key, index, obj);
						return;
					case "number":
						AddNumberInput(key, index, obj);
						return;
					default:
						Console.WriteLine("Unrecognized complex setting type: " + settingType, MessageType.Warning);
						return;
				}
			}

			Console.WriteLine("Unrecognized setting type: " + value.GetType(), MessageType.Error);
		}

		private void AddToggleInput(string key, int index, JObject obj = null)
		{
			var toggle = AddToggleInput(_toggleTemplate.Copy(key), index);
			toggle.Element.name = key;
			toggle.Title = (string)obj?["title"] ?? key;
			toggle.Show();
		}

		private void AddSliderInput(string key, int index, JObject obj)
		{
			var slider = AddSliderInput(_sliderTemplate.Copy(key), index);
			slider.Min = (float)obj["min"];
			slider.Max = (float)obj["max"];
			slider.Element.name = key;
			slider.Title = (string)obj["title"] ?? key;
			slider.Show();
		}

		private void AddSelectorInput(string key, int index, JObject obj)
		{
			var options = obj["options"].ToObject<string[]>();
			var selector = AddSelectorInput(_selectorTemplate.Copy(key), index);
			selector.Element.name = key;
			selector.Title = (string)obj["title"] ?? key;
			selector.Initialize((string)obj["value"], options);
			selector.Show();
		}

		private void AddTextInput(string key, int index, JObject obj = null)
		{
			var textInput = AddTextInput(_textInputTemplate.Copy(key), index);
			textInput.Element.name = key;
			textInput.Title = (string)obj?["title"] ?? key;
			textInput.Show();
		}

		private void AddNumberInput(string key, int index, JObject obj = null)
		{
			var numberInput = AddNumberInput(_numberInputTemplate.Copy(key), index);
			numberInput.Element.name = key;
			numberInput.Title = (string)obj?["title"] ?? key;
			numberInput.Show();
		}

		private void AddSeparator(string key, int index, JObject obj)
		{
			var numberInput = AddSeparator(_seperatorTemplate.Copy("Inputs"), index);
			numberInput.Element.name = key;
			numberInput.Title = (string)obj?["title"] ?? key;
			numberInput.Show();
		}
	}
}
