namespace OWML.Common
{
    public interface ISocketMessage
    {
        string SenderName { get; }
        string SenderType { get; }
        MessageType Type { get; }
        string Message { get; }
    }
}