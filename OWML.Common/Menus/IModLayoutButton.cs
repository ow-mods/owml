using UnityEngine;
using UnityEngine.UI;

namespace OWML.Common.Menus
{
    public interface IModLayoutButton : IModButton
    {
        HorizontalLayoutGroup LayoutGroup { get; }
        void UpdateState();
        void AddText(string text);
        void AddPicture(Texture2D texture, float scale = 1.0f);
    }
}