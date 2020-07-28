using System;

namespace OWML.Common
{
    public interface IModConsole
    {
        [Obsolete("Use WriteLine(string line) or WriteLine(string line, MessageType) instead.")]
        void WriteLine(params object[] objects);

        void WriteLine(string line);
        void WriteLine(string line, MessageType type);
    }
}
