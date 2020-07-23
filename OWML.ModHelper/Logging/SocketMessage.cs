using Newtonsoft.Json;
using OWML.Common;

namespace OWML.ModHelper
{
    public class SocketMessage
    {
        [JsonProperty("senderName")]
        public string SenderName;
        [JsonProperty("senderType")]
        public string SenderType;
        [JsonProperty("type")]
        public MessageType Type;
        [JsonProperty("message")]
        public string Message;
    }
}
