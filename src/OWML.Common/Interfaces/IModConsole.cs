using System;

namespace OWML.Common
{
    public interface IModConsole
    {
        [Obsolete("Use WriteLine(string) or WriteLine(string, MessageType) instead.")]
        void WriteLine(params object[] objects);

        void WriteLine(string line);
        void WriteLine(string line, MessageType type);
    }
}
