using System.IO;
using Newtonsoft.Json;

namespace OWML.ModHelper
{
    public static class JsonHelper
    {
        public static T LoadJsonObject<T>(string path)
        {
            if (!File.Exists(path))
            {
                return default;
            }
            var json = File.ReadAllText(path)
                .Replace("\\\\", "/")
                .Replace("\\", "/");
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static void SaveJsonObject<T>(string path, T obj)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}
