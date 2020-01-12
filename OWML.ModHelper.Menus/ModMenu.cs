using System;
using System.Collections.Generic;
using System.Linq;
using OWML.Common;
using OWML.ModHelper.Events;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModMenu : IModMenu
    {
        public Menu Menu { get; private set; }
        public List<IModButton> Buttons { get; }

        private readonly IModLogger _logger;
        private readonly IModConsole _console;
        public LayoutGroup LayoutGroup { get; private set; }

        public int ButtonOffset { get; private set; }

        public ModMenu(IModLogger logger, IModConsole console)
        {
            _logger = logger;
            _console = console;
            Buttons = new List<IModButton>();
        }

        public virtual void Initialize(Menu menu)
        {
            Menu = menu;
            LayoutGroup = Menu.GetComponent<LayoutGroup>() ?? Menu.GetComponentInChildren<LayoutGroup>();
            Buttons.AddRange(Menu.GetComponentsInChildren<Button>().Select(x => new ModButton(x, this)).Cast<IModButton>());
            ButtonOffset = Buttons.Any() ? Buttons[0].Button.transform.GetSiblingIndex() : 0;
        }

        [Obsolete("Use Buttons instead")]
        public List<Button> GetButtons()
        {
            return Menu.GetComponentsInChildren<Button>().ToList();
        }

        public IModButton GetButton(string title)
        {
            var button = Buttons.FirstOrDefault(x => x.Title == title);
            if (button == null)
            {
                _console.WriteLine("Warning: no button found with title: " + title);
            }
            return button;
        }

        [Obsolete("Use button.Copy() instead")]
        public Button AddButton(string title, int index)
        {
            _console.WriteLine("Adding main menu button: " + title);

            var original = Buttons?.FirstOrDefault();
            if (original == null)
            {
                _console.WriteLine("Warning: no buttons to copy");
                return null;
            }

            _logger.Log("Copying button: " + original.Title);

            var copy = original.Copy();
            copy.Title = title;
            copy.Index = index;

            AddButton(copy);

            return copy.Button;
        }

        public virtual void AddButton(IModButton button)
        {
            button.Button.transform.parent = LayoutGroup.transform;
            button.Index = button.Index;
            Buttons.Add(button);
        }

    }
}
