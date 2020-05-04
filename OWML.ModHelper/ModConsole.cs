using System;
using System.IO;
using System.Linq;
using System.Text;
using OWML.Common;

namespace OWML.ModHelper
{
    public class ModConsole : IModConsole
    {
        [Obsolete("Use ModHelper.Console instead")]
        public static ModConsole Instance { get; private set; }

        public static event Action<IModManifest, string> OnConsole;

        private static FileStream _writer;

        private readonly IModLogger _logger;
        private readonly IModManifest _manifest;

        public ModConsole(IOwmlConfig config, IModLogger logger, IModManifest manifest)
        {
            if (manifest.Name == "OWML")
            {
                Instance = this;
            }
            _logger = logger;
            _manifest = manifest;
            if (_writer == null)
            {
                _writer = File.Open(config.OutputFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            }
        }

        public void WriteLine(string s)
        {
            _logger.Log(s);
            OnConsole?.Invoke(_manifest, s);
            var message = $"[{_manifest.Name}]: {s}";
            InternalWriteLine(message);
        }

        public void WriteLine(params object[] objects)
        {
            WriteLine(string.Join(" ", objects.Select(o => o.ToString()).ToArray()));
        }

        private static void InternalWriteLine(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message + Environment.NewLine);
            _writer.Write(bytes, 0, bytes.Length);
            _writer.Flush();
        }

    }
}
