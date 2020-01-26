using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<IModButton> Buttons { get; private set; }
        public List<IModToggleInput> ToggleInputs { get; private set; }
        public List<IModSliderInput> SliderInputs { get; private set; }
        public List<IModTextInput> TextInputs { get; private set; }

        private readonly IModLogger _logger;
        private readonly IModConsole _console;
        private LayoutGroup _layoutGroup;

        public ModMenu(IModLogger logger, IModConsole console)
        {
            _logger = logger;
            _console = console;
        }

        public virtual void Initialize(Menu menu)
        {
            var root = menu.GetValue<GameObject>("_menuActivationRoot");
            var layoutGroup = root.GetComponent<LayoutGroup>() ?? root.GetComponentInChildren<LayoutGroup>();
            Initialize(menu, layoutGroup);
        }

        public virtual void Initialize(Menu menu, LayoutGroup layoutGroup)
        {
            Menu = menu;
            _layoutGroup = layoutGroup;
            Buttons = Menu.GetComponentsInChildren<Button>().Select(x => new ModButton(x, this)).Cast<IModButton>().ToList();
            ToggleInputs = Menu.GetComponentsInChildren<TwoButtonToggleElement>().Select(x => new ModToggleInput(x, this)).Cast<IModToggleInput>().ToList();
            SliderInputs = Menu.GetComponentsInChildren<SliderElement>().Select(x => new ModSliderInput(x, this)).Cast<IModSliderInput>().ToList();
            TextInputs = new List<IModTextInput>();
        }

        public IModButton GetButton(string title)
        {
            var button = Buttons.FirstOrDefault(x => x.Title == title || x.Button.name == title);
            if (button == null)
            {
                _console.WriteLine("Warning: no button found with title or name: " + title);
            }
            return button;
        }

        [Obsolete("Use Buttons instead")]
        public List<Button> GetButtons()
        {
            return Menu.GetComponentsInChildren<Button>().ToList();
        }

        [Obsolete("Use button.Duplicate instead")]
        public Button AddButton(string title, int index)
        {
            var original = Buttons?.FirstOrDefault();
            if (original == null)
            {
                _console.WriteLine("Warning: no buttons to copy");
                return null;
            }

            var copy = original.Copy();
            copy.Title = title;

            AddButton(copy, index);

            return copy.Button;
        }

        public IModButton AddButton(IModButton button)
        {
            return AddButton(button, button.Index);
        }

        public virtual IModButton AddButton(IModButton button, int index)
        {
            button.Button.transform.parent = _layoutGroup.transform;
            button.Index = index;
            button.Initialize(this);
            Buttons.Add(button);
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
            input.Element.transform.parent = _layoutGroup.transform;
            input.Index = index;
            ToggleInputs.Add(input);
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
            input.Element.transform.parent = _layoutGroup.transform;
            input.Index = index;
            SliderInputs.Add(input);
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
            input.Element.transform.parent = _layoutGroup.transform;
            input.Index = index;
            TextInputs.Add(input);
            return input;
        }

        public object GetInputValue(string key)
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
            var textInput = GetTextInput(key);
            if (textInput != null)
            {
                return textInput.Value;
            }
            _console.WriteLine("Error: no input found with name " + key);
            return null;
        }

        protected void InvokeOnInit()
        {
            OnInit?.Invoke();
        }

    }
}
