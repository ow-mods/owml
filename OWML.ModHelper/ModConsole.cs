using System;
using System.IO;
using System.Text;
using OWML.Common;

namespace OWML.ModHelper
{
    public class ModConsole : IModConsole
    {
        private readonly FileStream _writer;
        private readonly IModLogger _logger;

        public ModConsole(IModConfig config, IModLogger logger)
        {
            _logger = logger;
            _writer = File.Open(config.OutputFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
        }

        public void WriteLine(string s)
        {
            _logger.Log(s);
            var bytes = Encoding.UTF8.GetBytes(s + Environment.NewLine);
            _writer.Write(bytes, 0, bytes.Length);
            _writer.Flush();
        }

    }
}
