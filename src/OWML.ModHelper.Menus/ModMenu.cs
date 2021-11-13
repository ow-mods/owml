using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using OWML.Common;
using OWML.Common.Menus;
using OWML.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
	public class ModMenu : IModMenu
	{
		public event Action OnInit;

		public Menu Menu { get; protected set; }

		public List<IModButtonBase> BaseButtons { get; private set; }

		public List<IModButton> Buttons => BaseButtons.OfType<IModButton>().ToList();

		public List<IModLayoutButton> LayoutButtons => BaseButtons.OfType<IModLayoutButton>().ToList();

		public List<IModPromptButton> PromptButtons => BaseButtons.OfType<IModPromptButton>().ToList();

		public List<IModToggleInput> ToggleInputs => _inputs.OfType<IModToggleInput>().ToList();

		public List<IModSliderInput> SliderInputs => _inputs.OfType<IModSliderInput>().ToList();

		public List<IModSelectorInput> SelectorInputs => _inputs.OfType<IModSelectorInput>().ToList();

		public List<IModTextInput> TextInputs => _inputs.OfType<IModTextInput>().ToList();

		public List<IModNumberInput> NumberInputs => _inputs.OfType<IModNumberInput>().ToList();

		public List<IModSeparator> Separators { get; } = new();

		protected LayoutGroup Layout;
		protected IModConsole Console;

		private List<IModInputBase> _inputs;

		public ModMenu(IModConsole console) =>
			Console = console;

		public virtual void Initialize(Menu menu)
		{
			var root = menu.GetValue<GameObject>("_selectableItemsRoot") ??
					   menu.GetValue<GameObject>("_menuActivationRoot");
			var layoutGroup = root.GetComponent<LayoutGroup>() ??
							  root.GetComponentInChildren<LayoutGroup>(true);
			Initialize(menu, layoutGroup);
		}

		public virtual void Initialize(Menu menu, LayoutGroup layoutGroup)
		{
			Menu = menu;
			Layout = layoutGroup;

			var promptButtons = Menu.GetComponentsInChildren<ButtonWithHotkeyImageElement>(true).Select(x => x.GetComponent<Button>()).ToList();
			BaseButtons = new List<IModButtonBase>()
				.Concat(promptButtons.Select(x => new ModPromptButton(x, this, Console)))
				.Concat(Menu.GetComponentsInChildren<Button>(true).Except(promptButtons).Select(x => new ModTitleButton(x, this)))
				.ToList();

			_inputs = new List<IModInputBase>()
				.Concat(Menu.GetComponentsInChildren<ToggleElement>(true).Select(x => new ModToggleInput(x, this)))
				.Concat(Menu.GetComponentsInChildren<SliderElement>(true).Select(x => new ModSliderInput(x, this)))
				.Concat(Menu.GetComponentsInChildren<OptionsSelectorElement>(true).Select(x => new ModSelectorInput(x, this)))
				.ToList();
		}

		[Obsolete("Use GetTitleButton instead")]
		public IModButton GetButton(string title) =>
			GetTitleButton(title);

		public IModButton GetTitleButton(string title) =>
			GetTitleButton(title, Buttons);

		public IModPromptButton GetPromptButton(string title) =>
			GetTitleButton(title, PromptButtons);

		private T GetTitleButton<T>(string title, List<T> buttons) where T : IModButton
		{
			var button = buttons.FirstOrDefault(x => x.Title == title || x.Button.name == title);
			if (button == null)
			{
				Console.WriteLine("Warning - No button found with title or name: " + title, MessageType.Warning);
			}
			return button;
		}

		[Obsolete("Use AddButton(IModButtonBase) instead.")]
		public IModButton AddButton(IModButton button) =>
			AddButton(button, button.Index);

		[Obsolete("Use AddButton(IModButtonBase, int) instead.")]
		public virtual IModButton AddButton(IModButton button, int index) =>
			(IModButton)AddButton((IModButtonBase)button, index);

		public IModButtonBase AddButton(IModButtonBase button) =>
			AddButton(button, button.Index);

		public virtual IModButtonBase AddButton(IModButtonBase button, int index)
		{
			var transform = button.Button.transform;
			var scale = transform.localScale;
			transform.parent = Layout.transform;
			button.Index = index;
			button.Initialize(this);
			BaseButtons.Add(button);
			button.Button.transform.localScale = scale;
			return button;
		}

		public IModToggleInput GetToggleInput(string title) =>
			ToggleInputs.FirstOrDefault(x => x.Title == title || x.Element.name == title);

		public IModToggleInput AddToggleInput(IModToggleInput input) =>
			AddToggleInput(input, input.Index);

		public IModToggleInput AddToggleInput(IModToggleInput input, int index)
		{
			_inputs.Add(input);
			AddInput(input, index);
			return input;
		}

		public IModSliderInput GetSliderInput(string title) =>
			SliderInputs.FirstOrDefault(x => x.Title == title || x.Element.name == title);

		public IModSliderInput AddSliderInput(IModSliderInput input) =>
			AddSliderInput(input, input.Index);

		public IModSliderInput AddSliderInput(IModSliderInput input, int index)
		{
			_inputs.Add(input);
			AddInput(input, index);
			return input;
		}

		public IModSelectorInput GetSelectorInput(string title) =>
			SelectorInputs.FirstOrDefault(x => x.Title == title || x.Element.name == title);

		public IModSelectorInput AddSelectorInput(IModSelectorInput input) =>
			AddSelectorInput(input, input.Index);

		public IModSelectorInput AddSelectorInput(IModSelectorInput input, int index)
		{
			_inputs.Add(input);
			AddInput(input, index);
			return input;
		}

		public IModTextInput GetTextInput(string title) =>
			TextInputs.FirstOrDefault(x => x.Title == title || x.Element.name == title);

		public IModTextInput AddTextInput(IModTextInput input) =>
			AddTextInput(input, input.Index);

		public IModTextInput AddTextInput(IModTextInput input, int index)
		{
			_inputs.Add(input);
			AddInput(input, index);
			return input;
		}

		public IModNumberInput GetNumberInput(string title) =>
			NumberInputs.FirstOrDefault(x => x.Title == title || x.Element.name == title);

		public IModNumberInput AddNumberInput(IModNumberInput input) =>
			AddNumberInput(input, input.Index);

		public IModNumberInput AddNumberInput(IModNumberInput input, int index)
		{
			_inputs.Add(input);
			AddInput(input, index);
			return input;
		}

		private void AddInput<T>(IModInput<T> input, int index)
		{
			var transform = input.Element.transform;
			var scale = transform.localScale;
			transform.parent = Layout.transform;
			input.Index = index;
			input.Initialize(this);
			input.Element.transform.localScale = scale;
		}

		public IModSeparator AddSeparator(IModSeparator separator) =>
			AddSeparator(separator, separator.Index);

		public IModSeparator AddSeparator(IModSeparator separator, int index)
		{
			Separators.Add(separator);
			var transform = separator.Element.transform;
			var scale = transform.localScale;
			transform.parent = Layout.transform;
			separator.Index = index;
			separator.Initialize(this);
			transform.localScale = scale;
			return separator;
		}

		public IModSeparator GetSeparator(string title) =>
			Separators.FirstOrDefault(x => x.Title == title || x.Element.name == title);

		public T GetInputValue<T>(string key) =>
			(T)GetInputValue(key);

		public object GetInputValue(string key)
		{
			var slider = GetSliderInput(key);
			if (slider != null)
			{
				return slider.Value;
			}
			var selector = GetSelectorInput(key);
			if (selector != null)
			{
				return selector.Value;
			}
			var toggle = GetToggleInput(key);
			if (toggle != null)
			{
				return toggle.Value;
			}
			var textInput = GetTextInput(key);
			if (textInput != null)
			{
				return textInput.Value;
			}
			var numberInput = GetNumberInput(key);
			if (numberInput != null)
			{
				return numberInput.Value;
			}
			if (GetSeparator(key) == null)
			{
				Console.WriteLine($"Error - No input found with name {key}", MessageType.Error);
			}
			return null;
		}

		public void SetInputValue(string key, object value)
		{
			var slider = GetSliderInput(key);
			if (slider != null)
			{
				var val = value is JObject obj ? obj["value"] : value;
				slider.Value = Convert.ToSingle(val);
				return;
			}
			var selector = GetSelectorInput(key);
			if (selector != null)
			{
				var val = value is JObject obj ? obj["value"] : value;
				selector.Value = Convert.ToString(val);
				return;
			}
			var toggle = GetToggleInput(key);
			if (toggle != null)
			{
				var val = value is JObject obj ? obj["value"] : value;
				toggle.Value = Convert.ToBoolean(val);
				return;
			}
			var textInput = GetTextInput(key);
			if (textInput != null)
			{
				var val = value is JObject obj ? obj["value"] : value;
				textInput.Value = Convert.ToString(val);
				return;
			}
			var numberInput = GetNumberInput(key);
			if (numberInput != null)
			{
				var val = value is JObject obj ? obj["value"] : value;
				numberInput.Value = Convert.ToSingle(val);
				return;
			}
			if (GetSeparator(key) == null)
			{
				Console.WriteLine("Error - No input found with name " + key, MessageType.Error);
			}
		}

		protected void InvokeOnInit() =>
			OnInit?.Invoke();

		public virtual void SelectFirst()
		{
			var firstSelectable = Menu.GetComponentInChildren<Selectable>();
			//Locator.GetMenuInputModule().SelectOnNextUpdate(firstSelectable);
			Menu.SetSelectOnActivate(firstSelectable);
		}

		protected void UpdateNavigation(List<Selectable> selectables)
		{
			for (var i = 0; i < selectables.Count; i++)
			{
				var upIndex = (i - 1 + selectables.Count) % selectables.Count;
				var downIndex = (i + 1) % selectables.Count;
				var navigation = selectables[i].navigation;
				navigation.selectOnUp = selectables[upIndex];
				navigation.selectOnDown = selectables[downIndex];
				selectables[i].navigation = navigation;
			}
		}

		public virtual void UpdateNavigation()
		{
			var selectables = Menu.GetComponentsInChildren<MenuOption>()
				.Select(x => x.GetComponent<Selectable>())
				.Where(x => x != null).ToList();
			UpdateNavigation(selectables);
		}
	}
}
