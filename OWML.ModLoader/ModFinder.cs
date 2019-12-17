using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using OWML.Common;

namespace OWML.ModLoader
{
    public class ModFinder : IModFinder
    {
        private readonly IModConfig _config;
        private readonly IModLogger _logger;
        private readonly IModConsole _console;

        public ModFinder(IModConfig config, IModLogger logger, IModConsole console)
        {
            _config = config;
            _logger = logger;
            _console = console;
        }

        public IList<IModManifest> GetManifests()
        {
            var manifestFilenames = Directory.GetFiles(_config.ModsPath, "manifest.json", SearchOption.AllDirectories);
            var manifests = new List<IModManifest>();
            foreach (var manifestFilename in manifestFilenames)
            {
                var json = File.ReadAllText(manifestFilename);
                var manifest = JsonConvert.DeserializeObject<ModManifest>(json);
                manifest.FolderPath = manifestFilename.Substring(0, manifestFilename.IndexOf("manifest.json"));
                manifests.Add(manifest);
            }
            return manifests;
        }

        public Type LoadModType(IModManifest manifest)
        {
            if (!manifest.Enabled)
            {
                _logger.Log($"{manifest.UniqueName} is disabled, skipping");
                return null;
            }
            _logger.Log("Loading assembly: " + manifest.AssemblyPath);
            var assembly = Assembly.LoadFile(manifest.AssemblyPath);
            _logger.Log($"Loaded {assembly.FullName}");
            try
            {
                return assembly.GetTypes().FirstOrDefault(x => x.IsSubclassOf(typeof(ModBehaviour)));
            }
            catch (Exception ex)
            {
                _logger.Log($"Error while trying to get {typeof(ModBehaviour)}: {ex.Message}");
                _console.WriteLine($"Error while trying to get {typeof(ModBehaviour)}: {ex.Message}");
                return null;
            }
        }

    }
}
