using System.Collections.Generic;

namespace OWML.Common
{
	public interface IModSorter
	{
		IList<IModData> SortMods(IList<IModData> mods);
	}
}
