using System;
using OWML.Common;
using OWML.Common.Enums;
using OWML.Common.Interfaces;
using OWML.Common.Models;
using OWML.GameFinder;
using OWML.Logging;
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
            var consoleWriter = CreateConsoleWriter(owmlConfig, owmlManifest, hasConsolePort);
            var modFinder = new ModFinder(owmlConfig, consoleWriter);
            var pathFinder = new PathFinder(owmlConfig, consoleWriter);
            var owPatcher = new OWPatcher(owmlConfig, consoleWriter);
            var binaryPatcher = new BinaryPatcher(owmlConfig, consoleWriter);
            var vrFilePatcher = new VRFilePatcher(consoleWriter, binaryPatcher);
            var vrPatcher = new VRPatcher(owmlConfig, binaryPatcher, vrFilePatcher);
            var versionReader = new GameVersionReader(new BinaryPatcher(owmlConfig, consoleWriter));
            var versionHandler = new GameVersionHandler(versionReader, consoleWriter, owmlManifest);
            var app = new App(owmlConfig, owmlManifest, consoleWriter, modFinder,
                pathFinder, owPatcher, vrPatcher, versionHandler);
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

        private static IModConsole CreateConsoleWriter(IOwmlConfig owmlConfig, IModManifest owmlManifest, bool hasConsolePort)
        {
            return hasConsolePort
                ? new ModSocketOutput(owmlConfig, null, owmlManifest, new ModSocket(owmlConfig.SocketPort))
                : (IModConsole)new OutputWriter();
        }

    }
}
