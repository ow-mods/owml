using System.Collections.Generic;
using OWML.Common;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModsMenu : ModPopupMenu
    {
        private readonly IModLogger _logger;
        private readonly IModConsole _console;
        private readonly List<IModBehaviour> _registeredMods;

        public ModsMenu(IModLogger logger, IModConsole console) : base(logger, console)
        {
            _logger = logger;
            _console = console;
            _registeredMods = new List<IModBehaviour>();
        }

        public void Register(IModBehaviour modBehaviour)
        {
            _console.WriteLine("Registering " + modBehaviour.ModHelper.Manifest.UniqueName);
            _registeredMods.Add(modBehaviour);
            var button = AddButton(modBehaviour.ModHelper.Manifest.UniqueName, 0);
            button.onClick.AddListener(() =>
            {
                var config = ParseConfig(button);
                modBehaviour.ModHelper.Storage.Save(config, "config.json");
                modBehaviour.Configure(config);
            });
        }

        private IModConfig ParseConfig(Button button)
        {
            _console.WriteLine("todo: implement ModsMenu.ParseConfig");
            return new ModConfig();
        }
        
    }
}
