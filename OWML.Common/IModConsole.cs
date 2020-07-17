using System;

namespace OWML.Common
{
    public interface IModConsole
    {
        void WriteLine(string s);
        void WriteLine(params object[] s);
    }
}
