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
        public List<IModButton> Buttons { get; private set; }

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
            Menu = menu;
            _layoutGroup = Menu.GetComponent<LayoutGroup>() ?? Menu.GetComponentInChildren<LayoutGroup>();
            Buttons = new List<IModButton>();
            Buttons.AddRange(Menu.GetComponentsInChildren<Button>().Select(x => new ModButton(x, this)).Cast<IModButton>());
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

        public void AddButton(IModButton button)
        {
            AddButton(button, button.Index);
        }

        public virtual void AddButton(IModButton button, int index)
        {
            button.Button.transform.parent = _layoutGroup.transform;
            button.Index = index;
            button.Initialize(this);
            Buttons.Add(button);
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
