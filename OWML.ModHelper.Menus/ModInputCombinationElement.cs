using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Input;
using OWML.ModHelper.Events;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace OWML.ModHelper.Menus
{
    public class ModInputCombinationElement : ModToggleInput, IModInputCombinationElement
    {
        private const float ScaleDown = 0.75f;

        public ILayoutManager Layout { get; private set; }

        override public string Title
        {
            get => _combination;
            set
            {
                _combination = value;
                UpdateContents();
            }
        }

        private string _combination;
        private readonly GameObject _layoutObject;
        private readonly IModInputHandler _inputHandler;

        private static IModInputCombinationElementMenu _popupMenu;

        private void SetupButtons()
        {
            var commandObject = new GameObject();
            var commandComponent = commandObject.AddComponent<ModCommandListener>();
            commandComponent.Initialize(InputLibrary.interact);
            commandComponent.OnNewlyReleased += OnEditButton;
            YesButton.Title = "Edit";
            YesButton.OnClick += OnEditClick;

            var deleteCommand = new SingleAxisCommand();
            var deleteBindingGmpd = new InputBinding(JoystickButton.FaceUp);
            var deleteBindingKbrd = new InputBinding(KeyCode.Delete);
            deleteCommand.SetInputs(deleteBindingGmpd, deleteBindingKbrd);
            commandObject = new GameObject();
            var updater = commandObject.AddComponent<ModCommandUpdater>();
            updater.Initialize(deleteCommand);
            commandComponent = commandObject.AddComponent<ModCommandListener>();
            commandComponent.Initialize(deleteCommand);
            commandComponent.OnNewlyReleased += OnDeleteButton;
            NoButton.Title = "Delete";
            NoButton.OnClick += OnDeleteClick;
        }

        public ModInputCombinationElement(TwoButtonToggleElement toggle, IModMenu menu,
            IModInputCombinationElementMenu popupMenu, IModInputHandler inputHandler, string combination = "") :
            base(toggle, menu)
        {
            _inputHandler = inputHandler;
            _combination = combination;
            _layoutObject = toggle.GetComponentInChildren<HorizontalLayoutGroup>().transform.Find("LabelBlock").
                GetComponentInChildren<HorizontalLayoutGroup>().gameObject;
            var layoutGroup = _layoutObject.GetComponent<HorizontalLayoutGroup>();
            var scale = toggle.transform.localScale;

            SetupButtons();

            Initialize(menu);
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.spacing = 0f;
            var constantGraphics = _layoutObject.GetComponentsInChildren<Graphic>(true);
            _layoutObject.transform.GetChild(1).gameObject.SetActive(false);
            Layout = new LayoutManager(layoutGroup, MonoBehaviour.FindObjectOfType<UIStyleManager>(),
                ModUIStyleApplier.ReplaceStyleApplier(toggle.gameObject), scale, constantGraphics);
            UpdateContents();
            _popupMenu = popupMenu;
        }

        private void UpdateContents()
        {
            Layout.Clear();
            if (_combination != "")
            {
                var keyStrings = _combination.Split('+');
                for (var j = 0; j < keyStrings.Length; j++)
                {
                    AddKeySign(keyStrings[j]);
                    if (j < keyStrings.Length - 1)
                    {
                        Layout.AddTextAt("+", Layout.ChildCount - 1);
                    }
                }
            }
            Layout.UpdateState();
        }

        private void AddKeySign(string key)
        {
            Layout.AddPictureAt(
               _inputHandler.Textures.KeyTexture(key),
               Layout.ChildCount - 1, ScaleDown);
        }

        private void OnEditButton()
        {
            if (Toggle.GetValue<bool>("_amISelected"))
            {
                OnEditClick();
            }
        }

        private void OnEditClick()
        {
            EventSystem.current.SetSelectedGameObject(this.Toggle.gameObject);//make sure it gets selected after popup closes

            _popupMenu.OnConfirm += OnPopupMenuConfirm;
            _popupMenu.OnCancel += OnPopupMenuCancel;
            var name = (Menu is IModInputCombinationMenu) ? (Menu as IModInputCombinationMenu).Title : "";
            _popupMenu.Open(_combination, name, Menu as IModInputCombinationMenu, this);
        }

        private void OnPopupMenuCancel()
        {
            _popupMenu.OnConfirm -= OnPopupMenuConfirm;
            _popupMenu.OnCancel -= OnPopupMenuCancel;
        }

        private void OnPopupMenuConfirm(string combination)
        {
            OnPopupMenuCancel();
            _combination = combination;
            UpdateContents();
        }

        public void Destroy()
        {
            Layout.Clear();
            Layout.UpdateState();
            Title = "";
            Toggle.gameObject.SetActive(false);
            GameObject.Destroy(Toggle.gameObject);
        }

        public void DestroySelf()
        {
            Destroy();
            (Menu as IModInputCombinationMenu)?.RemoveCombinationElement(this);
        }

        private void OnDeleteButton()
        {
            if (Toggle.GetValue<bool>("_amISelected"))
            {
                OnDeleteClick();
            }
        }

        private void OnDeleteClick()
        {
            DestroySelf();
        }

        public new IModInputCombinationElement Copy()
        {
            return Copy(_combination);
        }

        public new IModInputCombinationElement Copy(string combination)
        {
            var copy = GameObject.Instantiate(Toggle);
            GameObject.Destroy(copy.GetComponentInChildren<LocalizedText>());
            return new ModInputCombinationElement(copy, Menu, _popupMenu, _inputHandler, combination);
        }
    }
}
