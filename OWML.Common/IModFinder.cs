using System;
using System.Collections.Generic;

namespace OWML.Common
{
    public interface IModFinder
    {
        IList<IModManifest> GetManifests();
        Type LoadModType(IModManifest manifest);
    }
}
