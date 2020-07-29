using System;
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
            var hasConsolePort = CommandLineArguments.HasArgument(Constants.ConsolePortArgument);
            SaveConsolePort(owmlConfig, hasConsolePort);
            SaveOwmlPath(owmlConfig);
            var owmlManifest = GetOwmlManifest();
            var writer = CreateWriter(owmlConfig, owmlManifest, hasConsolePort);
            var modFinder = new ModFinder(owmlConfig, writer);
            var pathFinder = new PathFinder(owmlConfig, writer);
            var owPatcher = new OWPatcher(owmlConfig, writer);
            var vrPatcher = new VRPatcher(owmlConfig, writer);
            var app = new App(owmlConfig, owmlManifest, writer, modFinder,
                pathFinder, owPatcher, vrPatcher);
            app.Run(args);
        }

        private static void SaveConsolePort(IOwmlConfig owmlConfig, bool hasConsolePort)
        {
            if (hasConsolePort)
            {
                var argument = CommandLineArguments.GetArgument(Constants.ConsolePortArgument);
                if (!int.TryParse(argument, out var port))
                {
                    ConsoleUtils.WriteByType(MessageType.Error, "Error - Bad port.");
                    return;
                }
                owmlConfig.SocketPort = port;
                JsonHelper.SaveJsonObject(Constants.OwmlConfigFileName, owmlConfig);
            }
            else
            {
                new SocketListener(owmlConfig).Init();
            }
        }

        private static IOwmlConfig GetOwmlConfig()
        {
            return JsonHelper.LoadJsonObject<OwmlConfig>(Constants.OwmlConfigFileName);
        }

        private static IOwmlConfig CreateOwmlConfig()
        {
            var config = JsonHelper.LoadJsonObject<OwmlConfig>(Constants.OwmlDefaultConfigFileName);
            JsonHelper.SaveJsonObject(Constants.OwmlConfigFileName, config);
            return config;
        }

        private static void SaveOwmlPath(IOwmlConfig owmlConfig)
        {
            owmlConfig.OWMLPath = AppDomain.CurrentDomain.BaseDirectory;
            JsonHelper.SaveJsonObject(Constants.OwmlConfigFileName, owmlConfig);
        }

        private static IModManifest GetOwmlManifest()
        {
            return JsonHelper.LoadJsonObject<ModManifest>(Constants.OwmlManifestFileName);
        }

        private static IModConsole CreateWriter(IOwmlConfig owmlConfig, IModManifest owmlManifest, bool hasConsolePort)
        {
            return hasConsolePort
                ? new ModSocketOutput(owmlConfig, null, owmlManifest)
                : (IModConsole)new OutputWriter();
        }

    }
}
