using Newtonsoft.Json.Linq;
using System.Linq;
using OWML.Common;
using OWML.Common.Menus;

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

		protected abstract void AddInputs();

		protected abstract void UpdateUIValues();

		protected ModConfigMenuBase(IModManifest manifest, IModStorage storage, IModConsole console)
			: base(console)
		{
			Manifest = manifest;
			Storage = storage;
		}

		public void Initialize(Menu menu, IModToggleInput toggleTemplate, IModSliderInput sliderTemplate,
			IModTextInput textInputTemplate, IModNumberInput numberInputTemplate,
			IModSelectorInput selectorTemplate)
		{
			_toggleTemplate = toggleTemplate;
			_sliderTemplate = sliderTemplate;
			_textInputTemplate = textInputTemplate;
			_numberInputTemplate = numberInputTemplate;
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
				switch ((string)obj["type"])
				{
					case "slider":
						AddSliderInput(key, obj, index);
						return;
					case "toggle":
						AddToggleInput(key, obj, index);
						return;
					case "selector":
						AddSelectorInput(key, obj, index);
						return;
					default:
						Console.WriteLine("Error - Unrecognized complex setting: " + value, MessageType.Error);
						return;
				}
			}

			Console.WriteLine("Error - Unrecognized setting type: " + value.GetType(), MessageType.Error);
		}

		private void AddToggleInput(string key, int index)
		{
			var toggle = AddToggleInput(_toggleTemplate.Copy(key), index);
			//toggle.YesButton.Title = "Yes";
			//toggle.NoButton.Title = "No";
			toggle.Element.name = key;
			toggle.Title = key;
			toggle.Show();
		}

		private void AddToggleInput(string key, JObject obj, int index)
		{
			var toggle = AddToggleInput(_toggleTemplate.Copy(key), index);
			//toggle.YesButton.Title = (string)obj["yes"];
			//toggle.NoButton.Title = (string)obj["no"];
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
			var options = obj["options"].ToObject<string[]>();
			var selector = AddSelectorInput(_selectorTemplate.Copy(key), index);
			selector.Element.name = key;
			selector.Title = (string)obj["title"] ?? key;
			selector.Initialize((string)obj["value"], options);
			selector.Show();
		}

		private void AddTextInput(string key, int index)
		{
			var textInput = AddTextInput(_textInputTemplate.Copy(key), index);
			textInput.Element.name = key;
			textInput.Show();
		}

		private void AddNumberInput(string key, int index)
		{
			var numberInput = AddNumberInput(_numberInputTemplate.Copy(key), index);
			numberInput.Element.name = key;
			numberInput.Show();
		}
	}
}
