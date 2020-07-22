using System;

namespace OWML.Common
{
    public interface IModConsole
    {
        [Obsolete]
        void WriteLine(string line);
        [Obsolete]
        void WriteLine(params object[] objects);

        void WriteLine(MessageType type, string line);
        void WriteLine(MessageType type, params object[] objects);
    }
}
