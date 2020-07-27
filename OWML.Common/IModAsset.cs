using System;

namespace OWML.Common
{
    public interface IModAsset<T>
    {
        event Action<T> Loaded;

        [Obsolete("Use Loaded instead.")]
        Action<T> OnLoaded { get; set; }
    }
}
