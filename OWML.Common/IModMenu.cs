using System.Collections.Generic;
using UnityEngine.UI;

namespace OWML.Common
{
    public interface IModMenu
    {
        List<Button> GetButtons();
        Button AddButton(string title, int index);
    }
}
