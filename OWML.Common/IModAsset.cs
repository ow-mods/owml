using System;

namespace OWML.Common
{
    public interface IModAsset<T>
    {
        Action<T> OnLoaded { get; set; }
    }
}
