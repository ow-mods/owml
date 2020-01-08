using System;
using System.IO;
using Newtonsoft.Json;
using OWML.Common;
using OWML.ModHelper;
using OWML.ModLoader;
using OWML.Patcher;
using OWML.Update;

namespace OWML.Launcher
{
    public class Program
    {
        static void Main(string[] args)
        {
            var owmlConfig = GetOwmlConfig();
            var writer = new OutputWriter();
            var modFinder = new ModFinder(owmlConfig, writer);
            var outputListener = new OutputListener(owmlConfig);
            var pathFinder = new PathFinder(owmlConfig, writer);
            var owPatcher = new OWPatcher(owmlConfig, writer);
            var vrPatcher = new VrPatcher(owmlConfig, writer);
            var update = new ModUpdate(writer);
            var app = new App(owmlConfig, writer, modFinder, outputListener, pathFinder, owPatcher, vrPatcher, update);
            app.Run();
        }

        private static IOwmlConfig GetOwmlConfig()
        {
            var json = File.ReadAllText("OWML.Config.json")
                .Replace("\\", "/");
            var config = JsonConvert.DeserializeObject<OwmlConfig>(json);
            config.OWMLPath = AppDomain.CurrentDomain.BaseDirectory;
            return config;
        }

    }
}
