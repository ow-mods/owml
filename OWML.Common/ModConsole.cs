using System;
using System.IO;
using System.Text;

namespace OWML.Common
{
    public class ModConsole : IModConsole
    {
        private readonly FileStream _writer;

        public ModConsole(IModConfig config)
        {
            _writer = File.Open(config.OutputFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
        }

        public void WriteLine(string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s + Environment.NewLine);
            _writer.Write(bytes, 0, bytes.Length);
            _writer.Flush();
        }

    }
}
