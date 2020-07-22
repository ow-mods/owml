using System;

namespace OWML.Common
{
    public interface IModConsole
    {
        [Obsolete]
        void WriteLine(params object[] objects);

        void WriteLine(string line, MessageType type);
    }
}
