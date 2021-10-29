using System;

namespace OWML.Common
{
	public interface IModAsset<out T>
	{
		event Action<T> Loaded;

		T Asset { get; }
	}
}
