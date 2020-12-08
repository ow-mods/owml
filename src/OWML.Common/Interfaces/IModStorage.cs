namespace OWML.Common.Interfaces
{
    public interface IModStorage
    {
        T Load<T>(string filename);
        void Save<T>(T obj, string filename);
    }
}
