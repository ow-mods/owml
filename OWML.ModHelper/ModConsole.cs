using System;
using OWML.Common;

namespace OWML.ModHelper
{
    public abstract class ModConsole : IModConsole
    {
        [Obsolete("Use ModHelper.Console instead")]
        public static ModConsole Instance { get; private set; }

        public static event Action<IModManifest, string> OnConsole;

        protected readonly IModLogger _logger;
        protected readonly IModManifest _manifest;
        protected readonly IOwmlConfig _owmlConfig;

        public abstract void WriteLine(string s);
        public abstract void WriteLine(params object[] s);

        public ModConsole(IOwmlConfig config, IModLogger logger, IModManifest manifest)
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
