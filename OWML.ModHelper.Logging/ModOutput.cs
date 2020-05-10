using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using OWML.Common;
using OWML.ModHelper.Logging;

namespace OWML.ModHelper
{
    public abstract class ModOutput : IModConsole
    {
        [Obsolete("Use ModHelper.Console instead")]
        public static ModOutput Instance { get; private set; }

        public static event Action<IModManifest, string> OnConsole;

        protected readonly IModLogger _logger;
        protected readonly IModManifest _manifest;
        protected readonly IOwmlConfig _owmlConfig;

        public abstract void WriteLine(string s);
        public abstract void WriteLine(params object[] s);

        public ModOutput(IOwmlConfig config, IModLogger logger, IModManifest manifest)
        {
            _logger = logger;
            _manifest = manifest;
            _owmlConfig = config;

            if (manifest.Name == "OWML")
            {
                Instance = this;
            }
        }

        internal static void CallWriteCallback(IModManifest manifest, string text)
        {
            OnConsole?.Invoke(manifest, text);
        }
    }
}
