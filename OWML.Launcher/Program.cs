using System;
using Autofac;
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
        public static void Main(string[] args)
        {
            var owmlConfig = GetOwmlConfig() ?? CreateOwmlConfig();
            var hasConsolePort = CommandLineArguments.HasArgument(Constants.ConsolePortArgument);
            SaveConsolePort(owmlConfig, hasConsolePort);
            SaveOwmlPath(owmlConfig);
            var owmlManifest = GetOwmlManifest();
            var consoleWriter = CreateConsoleWriter(owmlConfig, owmlManifest, hasConsolePort);

            var builder = new ContainerBuilder(); // todo move?

            builder.RegisterInstance(owmlConfig).As<IOwmlConfig>();
            builder.RegisterInstance(owmlManifest).As<IModManifest>();
            builder.RegisterInstance(consoleWriter).As<IModConsole>();

            builder.RegisterType<ModFinder>().As<IModFinder>();
            builder.RegisterType<PathFinder>().As<IPathFinder>();
            builder.RegisterType<OWPatcher>().As<IOWPatcher>();
            builder.RegisterType<BinaryPatcher>().As<IBinaryPatcher>();
            builder.RegisterType<VRFilePatcher>().As<IVRFilePatcher>();
            builder.RegisterType<VRPatcher>().As<IVRPatcher>();
            builder.RegisterType<GameVersionReader>().As<IGameVersionReader>();
            builder.RegisterType<GameVersionHandler>().As<IGameVersionHandler>();

            builder.RegisterType<App>();

            var container = builder.Build();

            var app = container.Resolve<App>();
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
                ? new ModSocketOutput(owmlConfig, null, owmlManifest, new ModSocket(owmlConfig))
                : (IModConsole)new OutputWriter();
        }

    }
}
