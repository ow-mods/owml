namespace OWML.Common
{
    public interface IModStorage
    {
        T Load<T>(string filename);
        void Save<T>(T obj, string filename);
    }
}
