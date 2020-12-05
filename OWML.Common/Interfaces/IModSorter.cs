using System.Collections.Generic;

namespace OWML.Common.Interfaces
{
    public interface IModSorter
    {
        IList<IModData> SortMods(IList<IModData> mods);
    }
}
