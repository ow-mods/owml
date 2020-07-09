using OWML.ModHelper.Events;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper
{
    public class ControllerButton : MonoBehaviour
    {
        private Button _button;
        private SingleAxisCommand _inputCommand;
        private Menu _menu;
        private Selectable _selectable;

        public void Init(SingleAxisCommand inputCommand, Menu menu, Selectable selectable)
        {
            _button = GetComponent<Button>();
            _inputCommand = inputCommand;
            _menu = menu;
            _selectable = selectable;
            var imageElement = GetComponent<ButtonWithHotkeyImageElement>();
            if (imageElement == null)
            {
                return;
            }
            var textId = GetComponentInChildren<LocalizedText>().GetValue<UITextType>("_textID");
            var title = UITextLibrary.GetString(textId);
            imageElement.SetPrompt(new ScreenPrompt(inputCommand, title));
        }

        private void Update()
        {
            if (_inputCommand != null &&
                _button != null &&
                (_selectable == null || _selectable == _menu.GetLastSelected()) &&
                OWInput.IsNewlyPressed(_inputCommand, InputMode.Menu))
            {
                _button.onClick.Invoke();
            }
        }
    }
}
