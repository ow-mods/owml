using System;

namespace OWML.Common
{
    public interface IModConsole
    {
        [Obsolete]
        void WriteLine(params object[] objects);
        [Obsolete]
        void WriteLine(string line);

        void WriteLine(MessageType type, string line);
    }
}
