namespace OWML.Common.Interfaces
{
    public interface IProcessHelper
    {
        void Start(string path, string[] args);

        void KillCurrentProcess();
    }
}