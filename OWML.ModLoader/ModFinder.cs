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

        public ModFinder(IModConfig config)
        {
            _config = config;
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

    }
}
