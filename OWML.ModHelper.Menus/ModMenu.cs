using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Events;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModMenu : IModMenu
    {
        public event Action OnInit;

        public Menu Menu { get; protected set; }
        public List<IModButtonBase> BaseButtons { get; private set; }
        public List<IModToggleInput> ToggleInputs { get; private set; }
        public List<IModSliderInput> SliderInputs { get; private set; }
        public List<IModSelectorInput> SelectorInputs { get; private set; }
        public List<IModTextInput> TextInputs { get; private set; }
        public List<IModComboInput> ComboInputs { get; private set; }
        public List<IModNumberInput> NumberInputs { get; private set; }
        public List<IModButton> Buttons => BaseButtons.OfType<IModButton>().ToList();
        public List<IModLayoutButton> LayoutButtons => BaseButtons.OfType<IModLayoutButton>().ToList();
        public List<IModPromptButton> PromptButtons => BaseButtons.OfType<IModPromptButton>().ToList();
        public List<IModSeparator> Separators { get; private set; }

        protected LayoutGroup Layout;
        protected readonly IModConsole OwmlConsole;

        public ModMenu(IModConsole console)
        {
            OwmlConsole = console;
        }

        public virtual void Initialize(Menu menu)
        {
            var root = menu.GetValue<GameObject>("_selectableItemsRoot") ?? menu.GetValue<GameObject>("_menuActivationRoot");
            var layoutGroup = root.GetComponent<LayoutGroup>() ?? root.GetComponentInChildren<LayoutGroup>(true);
            Initialize(menu, layoutGroup);
        }

        public virtual void Initialize(Menu menu, LayoutGroup layoutGroup)
        {
            Menu = menu;
            Layout = layoutGroup;

            var promptButtons = Menu.GetComponentsInChildren<ButtonWithHotkeyImageElement>(true)
                .Select(x => x.GetComponent<Button>()).ToList();
            BaseButtons = promptButtons.Select(x => new ModPromptButton(x, this)).Cast<IModButtonBase>().ToList();

            var ordinaryButtons = Menu.GetComponentsInChildren<Button>(true).Except(promptButtons);
            BaseButtons.AddRange(ordinaryButtons.Select(x => new ModTitleButton(x, this)).Cast<IModButtonBase>().ToList());

            ToggleInputs = Menu.GetComponentsInChildren<TwoButtonToggleElement>(true).Select(x => new ModToggleInput(x, this)).Cast<IModToggleInput>().ToList();
            SliderInputs = Menu.GetComponentsInChildren<SliderElement>(true).Select(x => new ModSliderInput(x, this)).Cast<IModSliderInput>().ToList();
            SelectorInputs = Menu.GetComponentsInChildren<OptionsSelectorElement>(true).Select(x => new ModSelectorInput(x, this)).Cast<IModSelectorInput>().ToList();
            TextInputs = new List<IModTextInput>();
            NumberInputs = new List<IModNumberInput>();
            ComboInputs = new List<IModComboInput>();
            Separators = new List<IModSeparator>();
        }

        [Obsolete("Use GetTitleButton instead")]
        public IModButton GetButton(string title)
        {
            return GetTitleButton(title);
        }

        public IModButton GetTitleButton(string title)
        {
            return GetTitleButton(title, Buttons);
        }

        public IModPromptButton GetPromptButton(string title)
        {
            return GetTitleButton(title, PromptButtons);
        }

        private T GetTitleButton<T>(string title, List<T> buttons) where T : IModButton
        {
            var button = buttons.FirstOrDefault(x => x.Title == title || x.Button.name == title);
            if (button == null)
            {
                OwmlConsole.WriteLine("Warning - No button found with title or name: " + title, MessageType.Warning);
            }
            return button;
        }

        [Obsolete("Use AddButton(IModButtonBase) instead.")]
        public IModButton AddButton(IModButton button)
        {
            return AddButton(button, button.Index);
        }

        [Obsolete("Use AddButton(IModButtonBase, int) instead.")]
        public virtual IModButton AddButton(IModButton button, int index)
        {
            return (IModButton)AddButton((IModButtonBase)button, index);
        }

        public IModButtonBase AddButton(IModButtonBase button)
        {
            return AddButton(button, button.Index);
        }

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

        public IModToggleInput GetToggleInput(string title)
        {
            return ToggleInputs.FirstOrDefault(x => x.Title == title || x.Element.name == title);
        }

        public IModToggleInput AddToggleInput(IModToggleInput input)
        {
            return AddToggleInput(input, input.Index);
        }

        public IModToggleInput AddToggleInput(IModToggleInput input, int index)
        {
            ToggleInputs.Add(input);
            AddInput(input, index);
            return input;
        }

        public IModSliderInput GetSliderInput(string title)
        {
            return SliderInputs.FirstOrDefault(x => x.Title == title || x.Element.name == title);
        }

        public IModSliderInput AddSliderInput(IModSliderInput input)
        {
            return AddSliderInput(input, input.Index);
        }

        public IModSliderInput AddSliderInput(IModSliderInput input, int index)
        {
            SliderInputs.Add(input);
            AddInput(input, index);
            return input;
        }

        public IModSelectorInput GetSelectorInput(string title)
        {
            return SelectorInputs.FirstOrDefault(x => x.Title == title || x.Element.name == title);
        }

        public IModSelectorInput AddSelectorInput(IModSelectorInput input)
        {
            return AddSelectorInput(input, input.Index);
        }

        public IModSelectorInput AddSelectorInput(IModSelectorInput input, int index)
        {
            SelectorInputs.Add(input);
            AddInput(input, index);
            return input;
        }

        public IModTextInput GetTextInput(string title)
        {
            return TextInputs.FirstOrDefault(x => x.Title == title || x.Element.name == title);
        }

        public IModTextInput AddTextInput(IModTextInput input)
        {
            return AddTextInput(input, input.Index);
        }

        public IModTextInput AddTextInput(IModTextInput input, int index)
        {
            TextInputs.Add(input);
            AddInput(input, index);
            return input;
        }

        public IModComboInput GetComboInput(string title)
        {
            return ComboInputs.FirstOrDefault(x => x.Title == title || x.Element.name == title);
        }

        public IModComboInput AddComboInput(IModComboInput input)
        {
            return AddComboInput(input, input.Index);
        }

        public IModComboInput AddComboInput(IModComboInput input, int index)
        {
            ComboInputs.Add(input);
            AddInput(input, index);
            return input;
        }

        public IModNumberInput GetNumberInput(string title)
        {
            return NumberInputs.FirstOrDefault(x => x.Title == title || x.Element.name == title);
        }

        public IModNumberInput AddNumberInput(IModNumberInput input)
        {
            return AddNumberInput(input, input.Index);
        }

        public IModNumberInput AddNumberInput(IModNumberInput input, int index)
        {
            NumberInputs.Add(input);
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

        public IModSeparator AddSeparator(IModSeparator separator)
        {
            return AddSeparator(separator, separator.Index);
        }

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

        public IModSeparator GetSeparator(string title)
        {
            return Separators.FirstOrDefault(x => x.Title == title || x.Element.name == title);
        }

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
            var comboInput = GetComboInput(key);
            if (comboInput != null)
            {
                return comboInput.Value;
            }
            var numberInput = GetNumberInput(key);
            if (numberInput != null)
            {
                return numberInput.Value;
            }
            OwmlConsole.WriteLine($"Error - No input found with name {key}", MessageType.Error);
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
            var comboInput = GetComboInput(key);
            if (comboInput != null)
            {
                var val = value is JObject obj ? obj["value"] : value;
                comboInput.Value = Convert.ToString(val);
                return;
            }
            var numberInput = GetNumberInput(key);
            if (numberInput != null)
            {
                var val = value is JObject obj ? obj["value"] : value;
                numberInput.Value = Convert.ToSingle(val);
                return;
            }
            OwmlConsole.WriteLine("Error - No input found with name " + key, MessageType.Error);
        }

        protected void InvokeOnInit()
        {
            OnInit?.Invoke();
        }

        public virtual void SelectFirst()
        {
            var firstSelectable = Menu.GetComponentInChildren<Selectable>();
            Locator.GetMenuInputModule().SelectOnNextUpdate(firstSelectable);
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
            var selectables = Menu.GetComponentsInChildren<TooltipSelectable>()
                .Select(x => x.GetComponent<Selectable>())
                .Where(x => x != null).ToList();
            UpdateNavigation(selectables);
        }

    }
}
