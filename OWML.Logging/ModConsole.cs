using System;
using OWML.Common;

namespace OWML.Logging
{
    public abstract class ModConsole : IModConsole
    {
        protected readonly IModLogger Logger;
        protected readonly IModManifest Manifest;
        protected readonly IOwmlConfig OwmlConfig;

        [Obsolete("Use WriteLine(string) or WriteLine(string, MessageType) instead.")]
        public abstract void WriteLine(params object[] objects);

        public abstract void WriteLine(string line);
        public abstract void WriteLine(string line, MessageType type);

        protected ModConsole(IOwmlConfig config, IModLogger logger, IModManifest manifest)
        {
            Logger = logger;
            Manifest = manifest;
            OwmlConfig = config;
        }
    }
}
