﻿using System;
using OWML.Common;

namespace OWML.ModHelper
{
    public abstract class ModConsole : IModConsole
    {
        [Obsolete("Use ModHelper.Console instead")]
        public static ModConsole Instance { get; private set; }

        public static event Action<IModManifest, string> OnConsole;

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