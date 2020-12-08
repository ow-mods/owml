using System.Collections.Generic;

namespace OWML.Common.Interfaces
{
    public interface IModFinder
    {
        IList<IModData> GetMods();
    }
}
