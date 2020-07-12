using OWML.Common.Menus;
using OWML.ModHelper.Events;
using OWML.ModHelper.Input;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public abstract class ModPopupInput<T> : ModInput<T>
    {
        protected readonly TwoButtonToggleElement ToggleElement;
        protected ModCommandListener CommandListener;

        public override bool IsSelected => ToggleElement.GetValue<bool>("_amISelected");

        private void SetupCommands()
        {
            var listenerObject = new GameObject();
            CommandListener = listenerObject.AddComponent<ModCommandListener>();
            CommandListener.AddToListener(InputLibrary.select);
            CommandListener.AddToListener(InputLibrary.enter);
            CommandListener.AddToListener(InputLibrary.enter2);
            CommandListener.OnNewlyPressed += OnOpenCommand;
        }

        protected ModPopupInput(TwoButtonToggleElement toggle, IModMenu menu) : base(toggle, menu)
        {
            ToggleElement = toggle;

            var noButton = ToggleElement.GetValue<Button>("_buttonFalse");
            noButton.transform.parent.gameObject.SetActive(false);

            var buttonParent = toggle.GetValue<Button>("_buttonTrue").transform.parent;
            var layoutGroup = buttonParent.parent.GetComponent<HorizontalLayoutGroup>();
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandWidth = true;

            buttonParent.GetComponent<LayoutElement>().preferredWidth = 100;

            SetupCommands();
        }

        protected void Subscribe(IModButton button)
        {
            button.OnClick += Open;
        }

        protected virtual void OnOpenCommand(SingleAxisCommand command)
        {
            if (IsSelected && (command == InputLibrary.select || command == InputLibrary.enter || command == InputLibrary.enter2))
            {
                command.ConsumeInput();
                command.BlockNextRelease();
                Open();
            }
        }

        protected abstract void Open();

    }
}
