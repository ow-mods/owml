using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace OWML.Common
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum MenuSide
	{
		[EnumMember(Value = @"left")] LEFT,
		[EnumMember(Value = @"center")] CENTER,
		[EnumMember(Value = @"right")] RIGHT
	}
}