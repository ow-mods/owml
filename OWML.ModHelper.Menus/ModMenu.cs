using System;
using System.Collections.Generic;
using System.Linq;
using OWML.Common;
using OWML.Common.Menus;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModMenu : IModMenu
    {
        public event Action OnInit;

        public Menu Menu { get; protected set; }
        public List<IModButton> Buttons { get; }
        public List<IModToggleInput> ToggleInputs { get; }
        public List<IModSliderInput> SliderInputs { get; }

        private readonly IModLogger _logger;
        private readonly IModConsole _console;
        private LayoutGroup _layoutGroup;

        public ModMenu(IModLogger logger, IModConsole console)
        {
            _logger = logger;
            _console = console;
            Buttons = new List<IModButton>();
            ToggleInputs = new List<IModToggleInput>();
            SliderInputs = new List<IModSliderInput>();
        }

        public virtual void Initialize(Menu menu)
        {
            var layoutGroup = menu.GetComponent<LayoutGroup>() ?? menu.GetComponentInChildren<LayoutGroup>();
            Initialize(menu, layoutGroup);
        }

        public virtual void Initialize(Menu menu, LayoutGroup layoutGroup)
        {
            Menu = menu;
            _layoutGroup = layoutGroup;
            Buttons.AddRange(Menu.GetComponentsInChildren<Button>().Select(x => new ModButton(x, this)).Cast<IModButton>());
            ToggleInputs.AddRange(Menu.GetComponentsInChildren<TwoButtonToggleElement>().Select(x => new ModToggleInput(x, this)).Cast<IModToggleInput>());
            SliderInputs.AddRange(Menu.GetComponentsInChildren<SliderElement>().Select(x => new ModSliderInput(x, this)).Cast<IModSliderInput>());
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

        public IModButton GetButton(string title)
        {
            var button = Buttons.FirstOrDefault(x => x.Title == title || x.Button.name == title);
            if (button == null)
            {
                _console.WriteLine("Warning: no button found with title or name: " + title);
            }
            return button;
        }

        protected void InvokeOnInit()
        {
            OnInit?.Invoke();
        }

    }
}
