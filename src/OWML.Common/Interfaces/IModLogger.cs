namespace OWML.Common
{
    public interface IModLogger
    {
        void Log(string s);

        void Log(params object[] s);
    }
}
