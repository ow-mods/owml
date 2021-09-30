using Newtonsoft.Json;
using OWML.Common;

namespace OWML.Logging
{
	public class ModSocketMessage : IModSocketMessage
	{
		[JsonProperty("senderName")]
		public string SenderName { get; set; }

		[JsonProperty("senderType")]
		public string SenderType { get; set; }

		[JsonProperty("type")]
		public MessageType Type { get; set; }

		[JsonProperty("message")]
		public string Message { get; set; }
	}
}