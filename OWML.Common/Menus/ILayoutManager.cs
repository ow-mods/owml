using UnityEngine;
using UnityEngine.UI;

namespace OWML.Common.Menus
{
    public interface ILayoutManager
    {
        LayoutGroup LayoutGroup { get; }
        int ChildCount { get; }

        void UpdateState();
        void Clear();
        void AddText(string text);
        void AddTextAt(string text, int index);
        void AddPicture(Texture2D texture, float scale = 1.0f);
        void AddPictureAt(Texture2D texture, int index, float scale = 1.0f);
    }
}
