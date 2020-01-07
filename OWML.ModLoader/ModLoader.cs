using System;
using System.IO;
using Newtonsoft.Json;
using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Menus;
using UnityEngine;

namespace OWML.ModLoader
{
    public class ModLoader
    {
        private static readonly string ConfigPath = $"{Application.dataPath}/Managed/OWML.Config.json";

        public static void LoadMods()
        {
            var owmlConfig = GetOwmlConfig();
            if (owmlConfig == null)
            {
                return;
            }
            var logger = new ModLogger(owmlConfig);
            logger.Log("Got config!");
            var console = new ModConsole(owmlConfig, logger);
            console.WriteLine("Mod loader has been initialized.");
            var modFinder = new ModFinder(owmlConfig, console);
            var menus = new ModMenus(logger, console);
            var owo = new Owo(modFinder, logger, console, owmlConfig, menus);
            owo.LoadMods();
        }

        private static IOwmlConfig GetOwmlConfig()
        {
            try
            {
                var json = File.ReadAllText(ConfigPath);
                return JsonConvert.DeserializeObject<OwmlConfig>(json);
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
