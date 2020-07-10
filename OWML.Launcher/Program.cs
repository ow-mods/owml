using System;
using System.IO;
using Newtonsoft.Json;
using OWML.Common;
using OWML.GameFinder;
using OWML.ModHelper;
using OWML.ModLoader;
using OWML.Patcher;

namespace OWML.Launcher
{
    public class Program
    {
        static void Main(string[] args)
        {
            var owmlConfig = GetOwmlConfig() ?? CreateOwmlConfig();
            owmlConfig.OWMLPath = AppDomain.CurrentDomain.BaseDirectory;
            var owmlManifest = GetOwmlManifest();
            var writer = OutputFactory.CreateOutput(owmlConfig, null, owmlManifest);
            var modFinder = new ModFinder(owmlConfig, writer);
            var outputListener = new OutputListener(owmlConfig);
            var pathFinder = new PathFinder(owmlConfig, writer);
            var owPatcher = new OWPatcher(owmlConfig, writer);
            var vrPatcher = new VRPatcher(owmlConfig, writer);
            var app = new App(owmlConfig, owmlManifest, writer, modFinder,
                outputListener, pathFinder, owPatcher, vrPatcher);
            app.Run(args);
        }

        private static IOwmlConfig GetOwmlConfig()
        {
            return GetJsonObject<OwmlConfig>(Constants.OwmlConfigFileName);
        }

        private static IOwmlConfig CreateOwmlConfig()
        {
            var config = GetJsonObject<OwmlConfig>(Constants.OwmlDefaultConfigFileName);
            var json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(Constants.OwmlConfigFileName, json);
            return config;
        }

        private static IModManifest GetOwmlManifest()
        {
            return GetJsonObject<ModManifest>(Constants.OwmlManifestFileName);
        }

        private static T GetJsonObject<T>(string filename)
        {
            if (!File.Exists(filename))
            {
                return default(T);
            }
            var json = File.ReadAllText(filename)
                .Replace("\\\\", "/")
                .Replace("\\", "/");
            return JsonConvert.DeserializeObject<T>(json);
        }

    }
}
