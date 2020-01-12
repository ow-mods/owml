using System;
using UnityEngine.UI;

namespace OWML.Common
{
    public interface IModButton
    {
        Action OnClick { get; set; }
        string Title { get; set; }
        int Index { get; set; }
        Button Button { get; }
        IModButton Copy();
        IModButton Duplicate();
        IModButton Replace();
    }
}