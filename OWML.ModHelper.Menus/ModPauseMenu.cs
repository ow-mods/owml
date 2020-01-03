using System;
using System.Collections.Generic;
using System.Linq;
using OWML.Common;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModPauseMenu : IModMenu
    {
        private readonly IModConsole _console;
        private readonly Menu _menu;

        public ModPauseMenu(IModConsole console)
        {
            _console = console;
            _menu = GameObject.Find("DCPauseMenu").GetComponent<Menu>();
        }

        public List<Button> GetButtons()
        {
            return _menu.GetComponentsInChildren<Button>().ToList();
        }

        public Button AddButton(string name, int index)
        {
            _console.WriteLine("Adding pause item: " + name);
            throw new NotImplementedException();
        }

    }
}
