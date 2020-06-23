using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OWML.Common.Menus
{
    public interface IModInputCombinationElement : IModToggleInput
    {
        ILayoutManager Layout { get; }
        void Destroy();
        void DestroySelf();
        new IModInputCombinationElement Copy(string combination);
        new string Title { get; set; }
    }
}
