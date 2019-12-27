using System.IO;
using Newtonsoft.Json;
using OWML.Common;

namespace OWML.ModHelper
{
    public class ModStorage : IModStorage
    {
        private readonly IModManifest _manifest;

        public ModStorage(IModManifest manifest)
        {
            _manifest = manifest;
        }

        public T Load<T>(string filename)
        {
            var path = _manifest.FolderPath + filename;
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public void Save<T>(T obj, string filename)
        {
            var path = _manifest.FolderPath + filename;
            var json = JsonConvert.SerializeObject(obj);
            File.WriteAllText(path, json);
        }
    }
}
