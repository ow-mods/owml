using System;
using System.IO;
using Newtonsoft.Json;
using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Events;
using UnityEngine;

namespace OWML.ModLoader
{
    public class ModLoader
    {
        private static readonly string ConfigPath = $"{Application.dataPath}/Managed/OWML.Config.json";

        public static void LoadMods()
        {
            var config = GetConfig();
            if (config == null)
            {
                return;
            }
            var logger = new ModLogger(config);
            logger.Log("Got config!");
            var console = new ModConsole(config, logger);
            console.WriteLine("Mod loader has been initialized.");
            var modFinder = new ModFinder(config);
            var owo = new Owo(modFinder, logger, console, config);
            owo.LoadMods();
        }

        private static IModConfig GetConfig()
        {
            try
            {
                var json = File.ReadAllText(ConfigPath);
                return JsonConvert.DeserializeObject<ModConfig>(json);
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
