using UnityEngine.UI;

namespace OWML.Common.Interfaces.Menus
{
    public interface IModSeparator : IModInputBase
    {
        LayoutElement LayoutElement { get; }

        IModSeparator Copy();
        IModSeparator Copy(string title);
    }
}