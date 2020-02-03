using OWML.ModHelper.Events;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper
{
    public class ControllerButton : MonoBehaviour
    {
        private Button _button;
        private SingleAxisCommand _inputCommand;

        public void Init(SingleAxisCommand inputCommand)
        {
            _button = GetComponent<Button>();
            _inputCommand = inputCommand;
            var imageElement = GetComponent<ButtonWithHotkeyImageElement>();
            if (imageElement != null)
            {
                var textId = GetComponentInChildren<LocalizedText>().GetValue<UITextType>("_textID");
                var title = UITextLibrary.GetString(textId);
                imageElement.SetPrompt(new ScreenPrompt(inputCommand, title), InputMode.Menu);
            }
        }

        private void Update()
        {
            if (_inputCommand != null && _button != null && OWInput.IsPressed(_inputCommand, InputMode.Menu))
            {
                _button.onClick.Invoke();
            }
        }

    }
}
