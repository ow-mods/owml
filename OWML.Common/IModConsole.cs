using System;

namespace OWML.Common
{
    public interface IModConsole
    {
        [Obsolete("Use IModConsole.Writeline(MessageType type, string s) instead")]
        void WriteLine(string s);
        [Obsolete("Use IModConsole.Writeline(MessageType type, params object[] s) instead")]
        void WriteLine(params object[] s);

        void WriteLine(MessageType type, string s);
        void WriteLine(MessageType type, params object[] s);
    }
}
