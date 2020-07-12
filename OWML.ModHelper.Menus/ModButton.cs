using System;
using OWML.Common.Menus;
using OWML.ModHelper.Events;
using Object = UnityEngine.Object;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public abstract class ModButton : IModButton
    {
        public event Action OnClick;

        public Button Button { get; }
        public IModMenu Menu { get; private set; }

        private int _index;
        public int Index
        {
            get => Button.transform.parent == null ? _index : Button.transform.GetSiblingIndex();
            set
            {
                _index = value;
                Button.transform.SetSiblingIndex(value);
            }
        }
        public bool IsSelected => _uIStyleApplier?.GetValue<bool>("_selected") ?? false;

        private readonly UIStyleApplier _uIStyleApplier;

        protected ModButton(Button button, IModMenu menu)
        {
            _uIStyleApplier = button.GetComponent<UIStyleApplier>();
            Button = button;
            Button.onClick.AddListener(() => OnClick?.Invoke());
            Initialize(menu);
        }

        public IModButton Copy()
        {
            var button = Object.Instantiate(Button);
            Object.Destroy(button.GetComponent<SubmitAction>());
            var modButton = (IModButton)Activator.CreateInstance(GetType(), button, Menu);
            modButton.Index = Index + 1;
            return modButton;
        }

        public void Initialize(IModMenu menu)
        {
            Menu = menu;
        }

        public IModButton Copy(int index)
        {
            var copy = Copy();
            copy.Index = index;
            return copy;
        }

        public IModButton Duplicate()
        {
            var copy = Copy();
            Menu.AddButton(copy);
            return copy;
        }

        public IModButton Duplicate(int index)
        {
            var dupe = Duplicate();
            dupe.Index = index;
            return dupe;
        }

        public IModButton Replace()
        {
            var duplicate = Duplicate();
            Hide();
            return duplicate;
        }

        public IModButton Replace(int index)
        {
            var replacement = Replace();
            replacement.Index = index;
            return replacement;
        }

        public void Show()
        {
            Button.gameObject.SetActive(true);
        }

        public void Hide()
        {
            Button.gameObject.SetActive(false);
        }

        [Obsolete("Use ModPromptButton and ModCommandListener instead")]
        public virtual void SetControllerCommand(SingleAxisCommand inputCommand)
        {
            ModConsole.Instance.WriteLine("Error: incompatible Button type");
        }

    }
}
