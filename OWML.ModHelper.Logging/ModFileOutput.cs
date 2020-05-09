using System;
using System.IO;
using System.Linq;
using System.Text;
using OWML.Common;

namespace OWML.ModHelper.Logging
{
    public class ModFileOutput : ModConsole
    {
        private static FileStream _writer;

        public ModFileOutput(IOwmlConfig config, IModLogger logger, IModManifest manifest) : base(config, logger, manifest)
        {
            if (_writer == null)
            {
                _writer = File.Open(config.OutputFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            }
        }

        public override void WriteLine(string s)
        {
            _logger.Log(s);
            CallWriteCallback(_manifest, s);
            var message = $"[{_manifest.Name}]: {s}";
            InternalWriteLine(message);
        }

        public override void WriteLine(params object[] objects)
        {
            WriteLine(string.Join(" ", objects.Select(o => o.ToString()).ToArray()));
        }

        private void InternalWriteLine(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message + Environment.NewLine);
            _writer.Write(bytes, 0, bytes.Length);
            _writer.Flush();
        }

    }
}
