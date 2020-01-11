using System.Collections.Generic;

namespace OWML.Common
{
    public interface IModFinder
    {
        IList<IModData> GetMods();
    }
}
