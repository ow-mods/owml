using System;
using System.Collections.Generic;

namespace OWML.Common
{
	public interface IModInteraction
	{
		IList<IModBehaviour> GetMods();

		IList<IModBehaviour> GetDependants(string dependencyUniqueName);

		IList<IModBehaviour> GetDependencies(string uniqueName);

		IModBehaviour TryGetMod(string uniqueName);

		TInterface TryGetModApi<TInterface>(string uniqueName) where TInterface : class;

		[Obsolete("Use TryGetMod")]
		IModBehaviour GetMod(string uniqueName);

		[Obsolete("Use TryGetModApi")]
		TInterface GetModApi<TInterface>(string uniqueName) where TInterface : class;

		bool ModExists(string uniqueName);
	}
}
