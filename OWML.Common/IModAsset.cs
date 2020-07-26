using System;

namespace OWML.Common
{
    public interface IModAsset<T>
    {
        event Action<T> OnCompleted;

        [Obsolete("Use OnCompleted instead.")]
        Action<T> OnLoaded { get; set; }
    }
}
