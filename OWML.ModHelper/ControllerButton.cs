using OWML.ModHelper.Events;
using UnityEngine;

namespace OWML.ModHelper
{
    public class ControllerButton : MonoBehaviour
    {
        public void Init(SingleAxisCommand inputCommand)
        {
            var imageElement = GetComponent<ButtonWithHotkeyImageElement>();
            if (imageElement == null)
            {
                return;
            }
            var textId = GetComponentInChildren<LocalizedText>().GetValue<UITextType>("_textID");
            var title = UITextLibrary.GetString(textId);
            imageElement.SetPrompt(new ScreenPrompt(inputCommand, title));
        }
    }
}
