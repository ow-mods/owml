using Newtonsoft.Json;

namespace OWML.LoadCustomAssets
{
    public class SaveFile
    {
        [JsonProperty("numberOfDucks")]
        public int NumberOfDucks { get; set; }
    }
}
