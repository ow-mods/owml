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
        public List<IModInput<bool>> ToggleElements { get; }
        public List<IModInput<float>> SliderElements { get; }

        private readonly IModLogger _logger;
        private readonly IModConsole _console;
        private LayoutGroup _layoutGroup;

        public ModMenu(IModLogger logger, IModConsole console)
        {
            _logger = logger;
            _console = console;
            Buttons = new List<IModButton>();
            ToggleElements = new List<IModInput<bool>>();
            SliderElements = new List<IModInput<float>>();
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
            ToggleElements.AddRange(Menu.GetComponentsInChildren<TwoButtonToggleElement>().Select(x => new ModToggleElement(x)).Cast<IModInput<bool>>());
            SliderElements.AddRange(Menu.GetComponentsInChildren<SliderElement>().Select(x => new ModSliderElement(x)).Cast<IModInput<float>>());
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
