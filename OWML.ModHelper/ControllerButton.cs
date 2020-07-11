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
            if (imageElement == null)
            {
                return;
            }
            var textId = GetComponentInChildren<LocalizedText>(true).GetValue<UITextType>("_textID");
            var title = UITextLibrary.GetString(textId);
            imageElement.SetPrompt(new ScreenPrompt(inputCommand, title));
        }

        private void Update()
        {
            if (_inputCommand != null && _button != null && OWInput.IsNewlyPressed(_inputCommand, InputMode.Menu))
            {
                _button.onClick.Invoke();
            }
        }
    }
}
