using OWML.Common.Menus;
using OWML.ModHelper.Events;
using UnityEngine.UI;
using UnityEngine;
using OWML.ModHelper.Input;
using Object = UnityEngine.Object;
using System;

namespace OWML.ModHelper.Menus
{
    public class ModPromptButton : ModTitleButton, IModPromptButton
    {
        private ScreenPrompt _prompt;
        private readonly UITextType _textId;
        private readonly ButtonWithHotkeyImageElement _hotkeyButton;
        private ModCommandListener _commandListener;

        public string DefaultTitle => UITextLibrary.GetString(_textId);
        public ScreenPrompt Prompt
        {
            get => _prompt;
            set
            {
                _prompt = value;
                _hotkeyButton.SetPrompt(value);
                if (_prompt.GetText() != DefaultTitle)
                {
                    Object.Destroy(Button.GetComponentInChildren<LocalizedText>());
                }
            }
        }

        public override string Title
        {
            set
            {
                if (_prompt == null)
                {
                    Prompt = new ScreenPrompt(value);
                    return;
                }
                _prompt.SetText(value);
                if (value != DefaultTitle)
                {
                    Object.Destroy(Button.GetComponentInChildren<LocalizedText>());
                }
            }
        }

        public ModPromptButton(Button button, IModMenu menu) : base(button, menu)
        {
            _hotkeyButton = Button.GetComponent<ButtonWithHotkeyImageElement>();
            if (_hotkeyButton == null)
            {
                ModConsole.Instance.WriteLine("Error: can't setup ModPromptButton for this button");
                return;
            }
            _prompt = _hotkeyButton.GetValue<ScreenPrompt>("_screenPrompt");
            _textId = Button.GetComponentInChildren<LocalizedText>(true)?.GetValue<UITextType>("_textID") ?? UITextType.None;
        }

        [Obsolete("Use Prompt and ModCommandListener instead")]
        public override void SetControllerCommand(SingleAxisCommand inputCommand)
        {
            Prompt = new ScreenPrompt(inputCommand, DefaultTitle);
            if (_commandListener!=null)
            {
                _commandListener.gameObject.SetActive(false);
                Object.Destroy(_commandListener.gameObject);
            }
            var commandObject = new GameObject("PromptButton_Listener");
            _commandListener = commandObject.AddComponent<ModCommandListener>();
            _commandListener.AddToListener(inputCommand);
            _commandListener.OnNewlyPressed += OnControllerCommand;
        }

        private void OnControllerCommand(SingleAxisCommand command)
        {
            Button.onClick.Invoke();
        }
    }
}
