using System;

namespace OWML.Common
{
	public interface IModAsset<T>
	{
		event Action<T> Loaded;

		T Asset { get; }
	}
}
