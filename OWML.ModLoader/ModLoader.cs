using System;
using System.IO;
using Newtonsoft.Json;
using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Assets;
using OWML.ModHelper.Events;
using UnityEngine;

namespace OWML.ModLoader
{
    public class ModLoader
    {
        private static readonly string ConfigPath = $"{Application.dataPath}/Managed/OWML.Config.json";
        private static readonly string SecondaryLogPath = $"{Application.dataPath}/Resources/OWML.Output.txt";

        public static void LoadMods()
        {
            SecondaryLog($"In {nameof(ModLoader)}.{nameof(LoadMods)}!");
            SecondaryLog("Getting config...");
            var config = GetConfig();
            if (config == null)
            {
                SecondaryLog("Config is null");
                return;
            }
            SecondaryLog("Got config!");
            SecondaryLog("Loading mods...");
            try
            {
                var logger = new ModLogger(config);
                var console = new ModConsole(config);
                var harmonyHelper = new HarmonyHelper(logger, console);
                var events = new ModEvents(harmonyHelper);
                var assets = new ModAssets(console);
                var helper = new ModHelper.ModHelper(config, logger, console, events, harmonyHelper, assets);
                var modFinder = new ModFinder(config);
                var owo = new Owo(helper, modFinder);
                owo.LoadMods();
                SecondaryLog("Loaded mods");
            }
            catch (Exception ex)
            {
                SecondaryLog("Error while loading mods: " + ex);
            }
        }

        private static IModConfig GetConfig()
        {
            try
            {
                var json = File.ReadAllText(ConfigPath);
                return JsonConvert.DeserializeObject<ModConfig>(json);
            }
            catch (Exception ex)
            {
                SecondaryLog("Error while loading config: " + ex);
                return null;
            }
        }

        private static void SecondaryLog(string s)
        {
            File.AppendAllText(SecondaryLogPath, $"{DateTime.Now}: {s}{Environment.NewLine}");
        }

    }
}
