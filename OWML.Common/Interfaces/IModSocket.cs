namespace OWML.Common.Interfaces
{
    public interface IModSocket
    {
        void WriteToSocket(IModSocketMessage message);

        void Close();
    }
}
