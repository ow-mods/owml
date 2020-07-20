using System;

namespace OWML.Common
{
    public interface IModConsole
    {
        [Obsolete]
        void WriteLine(string s);
        [Obsolete]
        void WriteLine(params object[] s);

        void WriteLine(string sender, MessageType type, string s);
        void WriteLine(MessageType type, string s);
        void WriteLine(string sender, MessageType type, params object[] s);
        void WriteLine(MessageType type, params object[] s);
    }
}
