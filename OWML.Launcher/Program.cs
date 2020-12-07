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
using OWML.Utils;

namespace OWML.Launcher
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var container = CreateContainer(args);
            var app = container.Resolve<App>();
            app.Run();
        }

        public static Container CreateContainer(string[] args)
        {
            var owmlConfig = GetOwmlConfig() ?? CreateOwmlConfig();
            var argumentHelper = new ArgumentHelper(args);
            var hasConsolePort = argumentHelper.HasArgument(Constants.ConsolePortArgument);
            SaveConsolePort(owmlConfig, hasConsolePort, argumentHelper);
            SaveOwmlPath(owmlConfig);
            var owmlManifest = GetOwmlManifest();
            var consoleWriter = CreateConsoleWriter(owmlConfig, owmlManifest, hasConsolePort);

            return new Container()
                .Add(owmlConfig)
                .Add(owmlManifest)
                .Add(consoleWriter)
                .Add<IArgumentHelper>(argumentHelper)
                .Add<IModFinder, ModFinder>()
                .Add<IPathFinder, PathFinder>()
                .Add<IOWPatcher, OWPatcher>()
                .Add<IBinaryPatcher, BinaryPatcher>()
                .Add<IVRFilePatcher, VRFilePatcher>()
                .Add<IVRPatcher, VRPatcher>()
                .Add<IGameVersionReader, GameVersionReader>()
                .Add<IGameVersionHandler, GameVersionHandler>()
                .Add<IProcessHelper, ProcessHelper>()
                .Add<App>();
        }

        private static void SaveConsolePort(IOwmlConfig owmlConfig, bool hasConsolePort, IArgumentHelper argumentHelper)
        {
            if (hasConsolePort)
            {
                var argument = argumentHelper.GetArgument(Constants.ConsolePortArgument);
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
                ? new ModSocketOutput(owmlConfig, owmlManifest, null, new ModSocket(owmlConfig), new ProcessHelper()) // todo container?
                : (IModConsole)new OutputWriter();
        }
    }
}
