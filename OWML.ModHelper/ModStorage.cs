using System;
using System.IO;
using Newtonsoft.Json;
using OWML.Common;

namespace OWML.ModHelper
{
    public class ModStorage : IModStorage
    {
        private readonly IModLogger _logger;
        private readonly IModConsole _console;
        private readonly IModManifest _manifest;

        public ModStorage(IModLogger logger, IModConsole console, IModManifest manifest)
        {
            _logger = logger;
            _console = console;
            _manifest = manifest;
        }

        public T Load<T>(string filename)
        {
            var path = _manifest.ModFolderPath + filename;
            _logger.Log($"Loading {path}...");
            if (!File.Exists(path))
            {
                _logger.Log("File not found: " + path);
                return default;
            }
            try
            {
                var json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                _console.WriteLine($"Error while saving {filename}: {ex}");
                return default;
            }
        }

        public void Save<T>(T obj, string filename)
        {
            var path = _manifest.ModFolderPath + filename;
            _logger.Log($"Saving {path}...");
            try
            {
                var json = JsonConvert.SerializeObject(obj);
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                _console.WriteLine($"Error while loading {filename}: {ex}");
            }
        }

    }
}
