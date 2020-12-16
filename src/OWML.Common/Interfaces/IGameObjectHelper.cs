using System;

namespace OWML.Common
{
	public interface IGameObjectHelper
	{
		TInterface CreateAndAdd<TInterface, TBehaviour>(string name = null);

		TBehaviour CreateAndAdd<TBehaviour>(string name = null);

		TBehaviour CreateAndAdd<TBehaviour>(Type type, string name = null);
	}
}