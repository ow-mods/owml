namespace OWML.Common
{
    public interface IModSocketMessage
    {
        string SenderName { get; }
        string SenderType { get; }
        MessageType Type { get; }
        string Message { get; }
    }
}