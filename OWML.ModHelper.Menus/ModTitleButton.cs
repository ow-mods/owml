using OWML.Common.Menus;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModTitleButton : BaseButton, IModTitleButton
    {
        private readonly Text _text;
        public string Title
        {
            get => _text != null ? _text.text : "";
            set
            {
                GameObject.Destroy(Button.GetComponentInChildren<LocalizedText>());
                _text.text = value;
            }
        }

        public ModTitleButton(Button button, IModMenu menu) : base(button, menu)
        {
            _text = Button.GetComponentInChildren<Text>();
        }

        public override IBaseButton Copy()
        {
            var button = GameObject.Instantiate(Button);
            GameObject.Destroy(button.GetComponent<SubmitAction>());
            return new ModTitleButton(button, Menu)
            {
                Index = Index + 1
            };
        }

        public IModTitleButton Copy(string title)
        {
            var copy = (IModTitleButton)Copy();
            copy.Title = title;
            return copy;
        }

        public IModTitleButton Copy(string title, int index)
        {
            var copy = Copy(title);
            copy.Index = index;
            return copy;
        }

        public IModTitleButton Duplicate(string title)
        {
            var dupe = (IModTitleButton)Duplicate();
            dupe.Title = title;
            return dupe;
        }

        public IModTitleButton Duplicate(string title, int index)
        {
            var dupe = Duplicate(title);
            dupe.Index = index;
            return dupe;
        }

        public IModTitleButton Replace(string title)
        {
            var replacement = (IModTitleButton)Replace();
            replacement.Title = title;
            return replacement;
        }

        public IModTitleButton Replace(string title, int index)
        {
            var replacement = Replace(title);
            replacement.Index = index;
            return replacement;
        }

    }
}
