using System;
using System.IO;
using Newtonsoft.Json;
using OWML.Common;
using OWML.ModHelper;
using OWML.ModLoader;

namespace OWML.Launcher
{
    public class Program
    {
        static void Main(string[] args)
        {
            var config = GetConfig();
            var writer = new OutputWriter();
            var modFinder = new ModFinder(config, writer);
            var outputListener = new OutputListener(config);
            var pathFinder = new PathFinder(config, writer);
            var app = new App(config, writer, modFinder, outputListener, pathFinder);
            app.Run();
        }

        private static IOwmlConfig GetConfig()
        {
            var json = File.ReadAllText("OWML.Config.json")
                .Replace("\\", "/");
            var config = JsonConvert.DeserializeObject<OwmlConfig>(json);
            config.OWMLPath = AppDomain.CurrentDomain.BaseDirectory;
            return config;
        }

    }
}
