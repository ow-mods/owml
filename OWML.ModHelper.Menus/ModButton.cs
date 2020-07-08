using OWML.Common.Menus;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModButton : BaseButton, IModButton
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

        public ModButton(Button button, IModMenu menu) : base(button, menu)
        {
            _text = Button.GetComponentInChildren<Text>();
        }

    }
}
