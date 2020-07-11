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
            var path = _manifest.ModFolderPath + filename;
            return JsonHelper.LoadJsonObject<T>(path);
        }

        public void Save<T>(T obj, string filename)
        {
            var path = _manifest.ModFolderPath + filename;
            JsonHelper.SaveJsonObject(path, obj);
        }

    }
}
