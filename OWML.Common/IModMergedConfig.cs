using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OWML.Common
{
    public interface IModMergedConfig : IModConfig
    {
        void SaveToStorage();
    }
}
