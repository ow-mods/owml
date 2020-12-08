using OWML.Common.Enums;

namespace OWML.Common.Interfaces
{
    public interface IModSocketMessage
    {
        string SenderName { get; }
        string SenderType { get; }
        MessageType Type { get; }
        string Message { get; }
    }
}
