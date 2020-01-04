using System.Collections.Generic;
using UnityEngine.UI;

namespace OWML.Common
{
    public interface IModMenu
    {
        List<Button> GetButtons();
        Button AddButton(string name, int index);
    }
}
