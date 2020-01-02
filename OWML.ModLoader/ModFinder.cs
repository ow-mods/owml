using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using OWML.Common;
using OWML.ModHelper;

namespace OWML.ModLoader
{
    public class ModFinder : IModFinder
    {
        private readonly IModConfig _config;
        private readonly IModConsole _console;

        public ModFinder(IModConfig config, IModConsole console)
        {
            _config = config;
            _console = console;
        }

        public IList<IModManifest> GetManifests()
        {
            if (!Directory.Exists(_config.ModsPath))
            {
                _console.WriteLine("Warning: Mods folder not found!");
                return new List<IModManifest>();
            }
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

    }
}
