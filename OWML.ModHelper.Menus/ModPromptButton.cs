using OWML.Common.Menus;
using OWML.ModHelper.Events;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModPromptButton : ModButton, IModPromptButton
    {
        private ScreenPrompt _prompt;
        private UITextType _textID;
        private readonly ButtonWithHotkeyImageElement _hotkeyButton;

        public string DefaultTitle
        {
            get => UITextLibrary.GetString(_textID);
        }
        public ScreenPrompt Prompt
        {
            get => _prompt;
            set
            {
                _prompt = value;
                _hotkeyButton.SetPrompt(value);
            }
        }

        public ModPromptButton(Button button, IModMenu menu) : base(button, menu)
        {
            _hotkeyButton = Button.GetComponent<ButtonWithHotkeyImageElement>();
            _textID = Button.GetComponentInChildren<LocalizedText>(true).GetValue<UITextType>("_textID");
            if (_hotkeyButton == null)
            {
                ModConsole.Instance.WriteLine("Error: can't setup ModPromptButton for this button");
                return;
            }
        }

        public new IModPromptButton Copy()
        {
            var button = (IModPromptButton)base.Copy();
            button.Prompt = Prompt;
            return button;
        }

        public IModPromptButton Copy(string title)
        {
            var copy = Copy();
            copy.Prompt = Prompt;
            return copy;
        }

        public IModPromptButton Copy(string title, int index)
        {
            var copy = Copy(title);
            copy.Index = index;
            return copy;
        }

        public IModPromptButton Duplicate(string title)
        {
            var dupe = (IModPromptButton)Duplicate();
            dupe.Prompt = Prompt;
            return dupe;
        }

        public IModPromptButton Duplicate(string title, int index)
        {
            var dupe = Duplicate(title);
            dupe.Index = index;
            return dupe;
        }

        public IModPromptButton Replace(string title)
        {
            var replacement = (IModPromptButton)Replace();
            replacement.Prompt = Prompt;
            return replacement;
        }

        public IModPromptButton Replace(string title, int index)
        {
            var replacement = Replace(title);
            replacement.Index = index;
            return replacement;
        }

    }
}
