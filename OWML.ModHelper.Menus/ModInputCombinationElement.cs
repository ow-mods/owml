using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Input;
using UnityEngine;
using UnityEngine.UI;

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

        public ModInputCombinationElement(TwoButtonToggleElement toggle, IModMenu menu, IModInputCombinationElementMenu popupMenu, IModInputHandler inputHandler, string combination = "") : base(toggle, menu)
        {
            _inputHandler = inputHandler;
            _combination = combination;
            _layoutObject = toggle.transform.GetChild(1).GetChild(0).GetChild(1).gameObject;
            var layoutGroup = _layoutObject.GetComponent<HorizontalLayoutGroup>();
            var scale = toggle.transform.localScale;
            YesButton.Title = "Edit";
            YesButton.OnClick += () => OnEditClick();
            NoButton.Title = "Delete";
            NoButton.OnClick += () => OnDeleteClick();
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
               _inputHandler.Textures.KeyTexture(key)
                , Layout.ChildCount - 1, ScaleDown);
        }

        private void OnEditClick()
        {
            _popupMenu.OnConfirm += OnPopupMenuConfirm;
            _popupMenu.OnCancel += OnPopupMenuCancel;
            _popupMenu.Open(_combination);
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
            (Menu as IModInputCombinationMenu)?.CombinationElements.Remove(this);
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
