namespace OWML.Common
{
    public interface IModSocket
    {
        void Connect();
        void WriteToSocket(ISocketMessage message);
    }
}