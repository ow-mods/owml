using System;
using System.Net;
using System.Net.Sockets;
using OWML.Common;
using OWML.GameFinder;
using OWML.ModHelper;
using OWML.ModHelper.Logging;
using OWML.ModLoader;
using OWML.Patcher;

namespace OWML.Launcher
{
    public class Program
    {
        static void Main(string[] args)
        {
            var owmlConfig = GetOwmlConfig() ?? CreateOwmlConfig();
            SaveConsolePort(owmlConfig);
            SaveOwmlPath(owmlConfig);
            var owmlManifest = GetOwmlManifest();
            var writer = OutputFactory.CreateOutput(owmlConfig, null, owmlManifest);
            var modFinder = new ModFinder(owmlConfig, writer);
            var outputListener = new OutputListener(owmlConfig);
            var pathFinder = new PathFinder(owmlConfig, writer);
            var owPatcher = new OWPatcher(owmlConfig, writer);
            var vrPatcher = new VRPatcher(owmlConfig, writer);
            var app = new App(owmlConfig, owmlManifest, writer, modFinder,
                outputListener, pathFinder, owPatcher, vrPatcher);
            app.Run();
        }

        private static void SaveConsolePort(IOwmlConfig owmlConfig)
        {
            if (CommandLineArguments.HasArgument(Constants.ConsolePortArgument))
            {
                Console.WriteLine("Port found in arguments - writing to config.");
                var argument = CommandLineArguments.GetArgument(Constants.ConsolePortArgument);
                if (!int.TryParse(argument, out var port))
                {
                    Console.WriteLine("Error - bad port.");
                    return;
                }
                owmlConfig.SocketPort = port;
            }
            else
            {
                Console.WriteLine("Creating port...");
                TcpListener l = new TcpListener(IPAddress.Loopback, 0);
                l.Start();
                int port = ((IPEndPoint)l.LocalEndpoint).Port;
                l.Stop();
                Console.WriteLine("Creating listener on port " + port);
                owmlConfig.SocketPort = port;
            }
            JsonHelper.SaveJsonObject(Constants.OwmlConfigFileName, owmlConfig);

            var socketListener = new SocketListener(port);
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

    }
}
