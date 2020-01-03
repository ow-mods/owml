using System;
using System.Collections.Generic;
using System.Linq;
using OWML.Common;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModOptionsMenu : IModMenu
    {
        private readonly IModConsole _console;
        private readonly TabbedOptionMenu _menu;

        public ModOptionsMenu(IModConsole console)
        {
            _console = console;
            _menu = GameObject.FindObjectOfType<TabbedOptionMenu>();
        }

        public List<Button> GetButtons()
        {
            return _menu.GetComponentsInChildren<Button>().ToList();
        }

        public Button AddButton(string text, int index)
        {
            _console.WriteLine("Adding options button: " + text);
            throw new NotImplementedException();
        }

    }
}
