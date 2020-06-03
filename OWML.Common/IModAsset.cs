using System;

namespace OWML.Common
{
    public interface IModAsset<T>
    {
        /// <summary>Fired when the asset is loaded.</summary>
        Action<T> OnLoaded { get; set; }
    }
}
