using System;
using OWML.Common;

namespace OWML.ModHelper.Logging
{
    public abstract class ModConsole : IModConsole
    {
        [Obsolete("Use ModHelper.Console instead")]
        public static ModConsole Instance { get; private set; }

        public static event Action<IModManifest, string> OnConsole;

        protected readonly IModLogger Logger;
        protected readonly IModManifest Manifest;
        protected readonly IOwmlConfig OwmlConfig;

        [Obsolete("Use ModConsole.Writeline(MessageType type, string s) instead")]
        public abstract void WriteLine(string s);
        [Obsolete("Use ModConsole.Writeline(MessageType type, params object[] s) instead")]
        public abstract void WriteLine(params object[] s);

        public abstract void WriteLine(MessageType type, string s);
        public abstract void WriteLine(MessageType type, params object[] s);

        protected ModConsole(IOwmlConfig config, IModLogger logger, IModManifest manifest)
        {
            Logger = logger;
            Manifest = manifest;
            OwmlConfig = config;

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
