using OWML.Common.Menus;
using UnityEngine;

namespace OWML.ModHelper
{
    public class ControllerButton : MonoBehaviour
    {
        private IModButton _button;
        private SingleAxisCommand _inputCommand;

        public void Init(IModButton button, SingleAxisCommand inputCommand)
        {
            _button = button;
            ModConsole.Instance.WriteLine("controller support: " + button.Title);
            _inputCommand = inputCommand;
            GetComponent<ButtonWithHotkeyImageElement>().SetPrompt(new ScreenPrompt(inputCommand, button.Title), InputMode.Menu);
        }

        private void Update()
        {
            if (_inputCommand != null && _button != null && OWInput.IsPressed(_inputCommand, InputMode.Menu))
            {
                _button.Click();
            }
        }

    }
}
