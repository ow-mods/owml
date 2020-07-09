using System;
using UnityEngine.UI;

namespace OWML.Common.Menus
{
    public interface IModLayoutButton : IModButton
    {
        IModLayoutManager Layout { get; }
    }
}