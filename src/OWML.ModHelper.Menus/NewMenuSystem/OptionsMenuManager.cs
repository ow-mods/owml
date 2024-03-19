using OWML.Common;
using OWML.ModHelper.Menus.CustomInputs;
using OWML.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus.NewMenuSystem
{
	internal class OptionsMenuManager : IOptionsMenuManager
	{
		private readonly IModConsole _console;
		private readonly IModUnityEvents _unityEvents;
		private readonly IPopupMenuManager _popupMenuManager;

		public OptionsMenuManager(IModConsole console, IModUnityEvents unityEvents, IPopupMenuManager popupMenuManager)
		{
			_console = console;
			_unityEvents = unityEvents;
			_popupMenuManager = popupMenuManager;
		}

		public (Menu menu, TabButton button) CreateStandardTab(string name)
		{
			var existingMenu = Resources.FindObjectsOfTypeAll<Menu>().First(x => x.name == "GraphicsMenu").gameObject;

			var newMenu = Object.Instantiate(existingMenu);
			newMenu.transform.parent = existingMenu.transform.parent;
			newMenu.transform.localScale = Vector3.one;
			newMenu.transform.localPosition = Vector3.zero;
			newMenu.transform.localRotation = Quaternion.identity;
			var rt = newMenu.GetComponent<RectTransform>();
			var ert = existingMenu.GetComponent<RectTransform>();
			rt.anchorMin = ert.anchorMin;
			rt.anchorMax = ert.anchorMax;
			rt.anchoredPosition3D = ert.anchoredPosition3D;
			rt.offsetMin = ert.offsetMin;
			rt.offsetMax = ert.offsetMax;
			rt.sizeDelta = ert.sizeDelta;

			var menu = newMenu.GetComponent<Menu>();

			var tabButton = CreateTabButton(name, menu);

			var optionsMenu = LoadManager.GetCurrentScene() == OWScene.TitleScreen
				? GameObject.Find("TitleMenu").transform.Find("OptionsCanvas").Find("OptionsMenu-Panel").GetComponent<TabbedMenu>()
				: GameObject.Find("PauseMenu").transform.Find("OptionsCanvas").Find("OptionsMenu-Panel").GetComponent<TabbedMenu>();

			optionsMenu._subMenus = optionsMenu._subMenus.Add(menu);
			optionsMenu._menuTabs = optionsMenu._menuTabs.Add(tabButton);
			optionsMenu._tabSelectablePairs = optionsMenu._tabSelectablePairs.Add(
				new TabbedMenu.TabSelectablePair()
				{
					tabButton = tabButton,
					selectable = null
				});

			if (optionsMenu._tabSelectablesInitialized)
			{
				tabButton.OnTabSelect += optionsMenu.OnTabButtonSelected;
			}

			foreach (var item in menu._menuOptions)
			{
				Object.Destroy(item.gameObject);
			}

			Object.Destroy(menu.transform.Find("Scroll View").Find("Viewport").Find("Content").Find("GammaButtonPanel").gameObject);

			menu._menuOptions = new MenuOption[] { };
			menu._selectOnActivate = null;

			return (menu, tabButton);
		}

		public (TabbedSubMenu menu, TabButton button) CreateTabWithSubTabs(string name)
		{
			var existingTabbedSubMenu = Resources.FindObjectsOfTypeAll<TabbedSubMenu>().Single(x => x.name == "GameplayMenu").gameObject;

			var newSubMenu = Object.Instantiate(existingTabbedSubMenu);
			newSubMenu.transform.parent = existingTabbedSubMenu.transform.parent;
			newSubMenu.transform.localScale = Vector3.one;
			newSubMenu.transform.localPosition = Vector3.zero;
			newSubMenu.transform.localRotation = Quaternion.identity;
			var rectTransform = newSubMenu.GetComponent<RectTransform>();
			rectTransform.anchorMin = existingTabbedSubMenu.GetComponent<RectTransform>().anchorMin;
			rectTransform.anchorMax = existingTabbedSubMenu.GetComponent<RectTransform>().anchorMax;
			rectTransform.anchoredPosition3D = existingTabbedSubMenu.GetComponent<RectTransform>().anchoredPosition3D;
			rectTransform.offsetMin = existingTabbedSubMenu.GetComponent<RectTransform>().offsetMin;
			rectTransform.offsetMax = existingTabbedSubMenu.GetComponent<RectTransform>().offsetMax;
			rectTransform.sizeDelta = existingTabbedSubMenu.GetComponent<RectTransform>().sizeDelta;

			var tabbedSubMenu = newSubMenu.GetComponent<TabbedSubMenu>();

			var tabButton = CreateTabButton(name, tabbedSubMenu);

			var optionsMenu = LoadManager.GetCurrentScene() == OWScene.TitleScreen
				? GameObject.Find("TitleMenu").transform.Find("OptionsCanvas").Find("OptionsMenu-Panel").GetComponent<TabbedMenu>()
				: GameObject.Find("PauseMenu").transform.Find("OptionsCanvas").Find("OptionsMenu-Panel").GetComponent<TabbedMenu>();

			optionsMenu._subMenus = optionsMenu._subMenus.Add(tabbedSubMenu);
			optionsMenu._menuTabs = optionsMenu._menuTabs.Add(tabButton);
			optionsMenu._tabSelectablePairs = optionsMenu._tabSelectablePairs.Add(
				new TabbedMenu.TabSelectablePair()
				{
					tabButton = tabButton,
					selectable = null
				});

			if (optionsMenu._tabSelectablesInitialized)
			{
				tabButton.OnTabSelect += optionsMenu.OnTabButtonSelected;
			}

			Object.Destroy(tabbedSubMenu._subMenus[0].gameObject);
			Object.Destroy(tabbedSubMenu._subMenus[1].gameObject);
			Object.Destroy(tabbedSubMenu._subMenus[2].gameObject);
			tabbedSubMenu._subMenus = null;
			Object.Destroy(tabbedSubMenu._tabSelectablePairs[0].tabButton.gameObject);
			Object.Destroy(tabbedSubMenu._tabSelectablePairs[1].tabButton.gameObject);
			Object.Destroy(tabbedSubMenu._tabSelectablePairs[2].tabButton.gameObject);
			tabbedSubMenu._tabSelectablePairs = null;
			tabbedSubMenu._selectOnActivate = null;

			return (tabbedSubMenu, tabButton);
		}

		public void RemoveTab(Menu tab)
		{
			var optionsMenu = LoadManager.GetCurrentScene() == OWScene.TitleScreen
				? GameObject.Find("TitleMenu").transform.Find("OptionsCanvas").Find("OptionsMenu-Panel").GetComponent<TabbedMenu>()
				: GameObject.Find("PauseMenu").transform.Find("OptionsCanvas").Find("OptionsMenu-Panel").GetComponent<TabbedMenu>();

			var tabButton = optionsMenu._menuTabs.SingleOrDefault(x => x._tabbedMenu == tab);

			if (tabButton == null)
			{
				return;
			}

			optionsMenu._subMenus = optionsMenu._subMenus.Remove(tab);
			optionsMenu._menuTabs = optionsMenu._menuTabs.Remove(tabButton);
			var tempList = optionsMenu._tabSelectablePairs.ToList();
			tempList.RemoveAll(x => x.tabButton == tabButton);
			optionsMenu._tabSelectablePairs = tempList.ToArray();

			foreach (var item in tab._menuOptions)
			{
				Object.Destroy(item.gameObject);
			}

			RecalculateNavigation(tabButton.transform.parent.GetComponentsInChildren<Button>(true).Where(x => x != tabButton.GetComponent<Button>()).ToList());

			Object.Destroy(tab.gameObject);
			Object.Destroy(tabButton.gameObject);
		}

		public (Menu subTabMenu, TabButton subTabButton) AddSubTab(TabbedSubMenu menu, string name)
		{
			var existingTabbedSubMenu = Resources.FindObjectsOfTypeAll<TabbedSubMenu>().Single(x => x.name == "GameplayMenu").gameObject;

			var existingSubMenu = existingTabbedSubMenu.GetComponent<TabbedSubMenu>()._subMenus[0].gameObject;
			var existingSubMenuTabButton = existingTabbedSubMenu.GetComponent<TabbedSubMenu>()._tabSelectablePairs[0].tabButton.gameObject;

			var newSubMenuTabButton = Object.Instantiate(existingSubMenuTabButton);
			newSubMenuTabButton.transform.parent = menu.transform.Find("SubMenuTabs");
			newSubMenuTabButton.transform.localScale = Vector3.one;
			newSubMenuTabButton.transform.localPosition = Vector3.zero;
			newSubMenuTabButton.transform.localRotation = Quaternion.identity;
			newSubMenuTabButton.transform.SetSiblingIndex(newSubMenuTabButton.transform.parent.childCount - 2);
			newSubMenuTabButton.name = $"Button-{name}Tab";
			Object.Destroy(newSubMenuTabButton.GetComponentInChildren<LocalizedText>());
			newSubMenuTabButton.GetComponentInChildren<Text>().text = name;

			var newSubMenu = Object.Instantiate(existingSubMenu);
			newSubMenu.transform.parent = menu.transform;
			newSubMenu.transform.localScale = Vector3.one;
			newSubMenu.transform.localPosition = Vector3.zero;
			newSubMenu.name = $"Menu{name}";
			newSubMenu.transform.localRotation = Quaternion.identity;

			var rt = newSubMenu.GetComponent<RectTransform>();
			var ert = existingSubMenu.GetComponent<RectTransform>();
			rt.anchorMin = ert.anchorMin;
			rt.anchorMax = ert.anchorMax;
			rt.anchoredPosition3D = ert.anchoredPosition3D;
			rt.offsetMin = ert.offsetMin;
			rt.offsetMax = ert.offsetMax;
			rt.sizeDelta = ert.sizeDelta;

			if (menu._selectOnActivate == null)
			{
				menu._selectOnActivate = newSubMenuTabButton.GetComponent<Button>();
			}

			menu._subMenus = menu._subMenus == null
				? (new[] { newSubMenu.GetComponent<Menu>() })
				: menu._subMenus.Add(newSubMenu.GetComponent<Menu>());

			menu._tabSelectablePairs = menu._tabSelectablePairs == null
				? (new TabbedMenu.TabSelectablePair[] { new TabbedMenu.TabSelectablePair() { tabButton = newSubMenuTabButton.GetComponent<TabButton>() } })
				: menu._tabSelectablePairs.Add(new TabbedMenu.TabSelectablePair() { tabButton = newSubMenuTabButton.GetComponent<TabButton>() });

			newSubMenuTabButton.GetComponent<TabButton>()._tabbedMenu = newSubMenu.GetComponent<Menu>();

			RecalculateNavigation(menu._tabSelectablePairs.Select(x => x.tabButton.GetComponent<Button>()).ToList());

			foreach (var item in newSubMenu.GetComponent<Menu>()._menuOptions)
			{
				Object.Destroy(item.gameObject);
			}

			newSubMenu.GetComponent<Menu>()._menuOptions = new MenuOption[] { };

			return (newSubMenu.GetComponent<Menu>(), newSubMenuTabButton.GetComponent<TabButton>());
		}

		public void OpenOptionsAtTab(TabButton button)
		{
			var optionsMenu = LoadManager.GetCurrentScene() == OWScene.TitleScreen
				? GameObject.Find("TitleMenu").transform.Find("OptionsCanvas").Find("OptionsMenu-Panel").GetComponent<TabbedMenu>()
				: GameObject.Find("PauseMenu").transform.Find("OptionsCanvas").Find("OptionsMenu-Panel").GetComponent<TabbedMenu>();

			optionsMenu.EnableMenu(true);
			optionsMenu.SelectTabButton(button);
		}

		public IOWMLToggleElement AddCheckboxInput(Menu menu, string label, string tooltip, bool initialValue)
		{
			var existingCheckbox = Resources.FindObjectsOfTypeAll<TabbedSubMenu>()
				.Single(x => x.name == "GameplayMenu").transform
				.Find("MenuGameplayBasic")
				.Find("UIElement-InvertPlayerLook").gameObject;

			var newCheckbox = Object.Instantiate(existingCheckbox);
			newCheckbox.transform.parent = GetParentForAddedElements(menu);
			newCheckbox.transform.localPosition = Vector3.zero;
			newCheckbox.transform.localScale = Vector3.one;
			newCheckbox.transform.name = $"UIElement-{label}";
			newCheckbox.transform.localRotation = Quaternion.identity;

			Object.Destroy(newCheckbox.GetComponentInChildren<LocalizedText>());
			
			var oldCheckbox = newCheckbox.GetComponent<ToggleElement>();
			var customCheckboxScript = newCheckbox.AddComponent<OWMLToggleElement>();
			customCheckboxScript._tooltipTextType = UITextType.None;
			customCheckboxScript._label = oldCheckbox._label;
			customCheckboxScript._label.text = label;
			customCheckboxScript._overrideTooltipText = tooltip;
			customCheckboxScript._displayText = oldCheckbox._displayText;
			customCheckboxScript._displayText.text = label;
			customCheckboxScript._toggleGraphic = oldCheckbox._toggleGraphic;
			customCheckboxScript._toggleElementButton = oldCheckbox._toggleElementButton;

			customCheckboxScript.Initialize(initialValue);

			Object.Destroy(oldCheckbox);

			menu._menuOptions = menu._menuOptions.Add(customCheckboxScript);

			if (menu._selectOnActivate == null)
			{
				menu._selectOnActivate = newCheckbox.GetComponent<Selectable>();
			}

			var labelBlock = newCheckbox.transform.Find("HorizontalLayoutGroup").Find("LabelBlock").GetComponent<LayoutElement>();
			labelBlock.minWidth = 860;
			labelBlock.preferredWidth = -1;

			return customCheckboxScript;
		}

		public IOWMLTwoButtonToggleElement AddToggleInput(Menu menu, string label, string leftButtonString, string rightButtonString, string tooltip, bool initialValue)
		{
			var existingToggle = Resources.FindObjectsOfTypeAll<TabbedSubMenu>()
				.Single(x => x.name == "InputMenu").transform
				.Find("MenuGeneral")
				.Find("UIElement-ConfirmToggle").gameObject;

			var script = existingToggle.GetComponent<TwoButtonToggleElement>();
			var text = script._buttonTrue.GetComponent<UIStyleApplier>()._textItems[0];
			var prefab = text.transform.root.gameObject;

			var newToggle = Object.Instantiate(prefab);
			newToggle.transform.parent = GetParentForAddedElements(menu);
			newToggle.transform.localPosition = Vector3.zero;
			newToggle.transform.localScale = Vector3.one;
			newToggle.transform.name = $"UIElement-{label}";
			newToggle.transform.localRotation = Quaternion.identity;

			// no idea why, but the prefab doesnt have this
			newToggle.GetComponent<LayoutElement>().preferredHeight = 70;

			Object.Destroy(newToggle.GetComponentInChildren<LocalizedText>());

			var oldToggle = newToggle.GetComponent<TwoButtonToggleElement>();
			var newScript = newToggle.AddComponent<OWMLTwoButtonToggleElement>();
			newScript._tooltipTextType = UITextType.None;
			newScript._label = oldToggle._label;
			newScript._label.text = label;
			newScript._overrideTooltipText = tooltip;
			newScript.ButtonTrue = oldToggle._buttonTrue;
			newScript.ButtonFalse = oldToggle._buttonFalse;

			newScript.ButtonTrue.GetComponent<UIStyleApplier>()._textItems[0].text = leftButtonString;
			newScript.ButtonFalse.GetComponent<UIStyleApplier>()._textItems[0].text = rightButtonString;


			_unityEvents.FireOnNextUpdate(() => newScript.Initialize(initialValue ? 1 : 0));

			Object.Destroy(oldToggle);

			menu._menuOptions = menu._menuOptions.Add(newScript);

			if (menu._selectOnActivate == null)
			{
				menu._selectOnActivate = newToggle.GetComponent<Selectable>();
			}

			var labelBlock = newScript.transform.Find("HorizontalLayoutGroup").Find("LabelBlock").GetComponent<LayoutElement>();
			labelBlock.minWidth = 860;
			labelBlock.preferredWidth = -1;

			return newScript;
		}

		public IOWMLOptionsSelectorElement AddSelectorInput(Menu menu, string label, string[] options, string tooltip, bool loopsAround, int initialValue)
		{
			var existingSelector = Resources.FindObjectsOfTypeAll<TabbedSubMenu>()
				.Single(x => x.name == "GameplayMenu").transform
				.Find("MenuGameplayBasic")
				.Find("UIElement-ControllerProfile").gameObject;

			var newSelector = Object.Instantiate(existingSelector);
			newSelector.transform.localPosition = Vector3.zero;
			newSelector.transform.parent = GetParentForAddedElements(menu);
			newSelector.transform.localScale = Vector3.one;
			newSelector.transform.name = $"UIElement-{label}";
			newSelector.transform.localRotation = Quaternion.identity;

			Object.Destroy(newSelector.GetComponentInChildren<LocalizedText>());

			var oldSelector = newSelector.GetComponent<OptionsSelectorElement>();
			var newScript = newSelector.AddComponent<OWMLOptionsSelectorElement>();
			newScript._tooltipTextType = UITextType.None;
			newScript._label = oldSelector._label;
			newScript._label.text = label;
			newScript._overrideTooltipText = tooltip;
			newScript._loopAround = loopsAround;
			newScript._optionsList = options;
			newScript._displayText = oldSelector._displayText;
			newScript._optionsBoxStyleApplier = oldSelector._optionsBoxStyleApplier;
			newScript._selectOnLeft = oldSelector._selectOnLeft;
			newScript._selectOnRight = oldSelector._selectOnRight;

			_unityEvents.FireOnNextUpdate(() => newScript.Initialize(initialValue));

			Object.Destroy(oldSelector);

			menu._menuOptions = menu._menuOptions.Add(newScript);

			if (menu._selectOnActivate == null)
			{
				menu._selectOnActivate = newSelector.GetComponent<Selectable>();
			}

			newSelector.SetActive(true);

			var labelBlock = newScript.transform.Find("HorizontalLayoutGroup").Find("LabelBlock").GetComponent<LayoutElement>();
			labelBlock.minWidth = 860;
			labelBlock.preferredWidth = -1;

			return newScript;
		}

		public IOWMLSliderElement AddSliderInput(Menu menu, string label, float lower, float upper, string tooltip, float initialValue)
		{
			var existingSlider = Resources.FindObjectsOfTypeAll<TabbedSubMenu>()
				.Single(x => x.name == "GameplayMenu").transform
				.Find("MenuGameplayBasic")
				.Find("UIElement-LookSensitivity").gameObject;

			var newSlider = Object.Instantiate(existingSlider);
			newSlider.transform.parent = GetParentForAddedElements(menu);
			newSlider.transform.localPosition = Vector3.zero;
			newSlider.transform.localScale = Vector3.one;
			newSlider.transform.name = $"UIElement-{label}";
			newSlider.transform.localRotation = Quaternion.identity;

			Object.Destroy(newSlider.GetComponentInChildren<LocalizedText>());

			var oldScript = newSlider.GetComponent<SliderElement>();
			var newScript = newSlider.AddComponent<OWMLSliderElement>();
			newScript._label = oldScript._label;
			newScript._label.text = label;
			newScript._overrideTooltipText = tooltip;

			_unityEvents.FireOnNextUpdate(() => newScript.Initialize(initialValue, lower, upper));

			Object.Destroy(oldScript);

			menu._menuOptions = menu._menuOptions.Add(newScript);

			if (menu._selectOnActivate == null)
			{
				menu._selectOnActivate = newSlider.GetComponent<Selectable>();
			}

			var labelBlock = newScript.transform.Find("HorizontalLayoutGroup").Find("Panel-Label").GetComponent<LayoutElement>();
			labelBlock.minWidth = 860;
			labelBlock.preferredWidth = -1;

			return newScript;
		}

		public GameObject AddSeparator(Menu menu, bool dots)
		{
			var separatorObj = new GameObject("separator");
			var layoutElement = separatorObj.AddComponent<LayoutElement>();
			layoutElement.flexibleWidth = 1;
			layoutElement.preferredHeight = 70;

			separatorObj.transform.parent = GetParentForAddedElements(menu);
			separatorObj.transform.localScale = Vector3.one;

			if (!dots)
			{
				return separatorObj;
			}

			var dotsSprite = Resources.FindObjectsOfTypeAll<TabbedSubMenu>()
				.Single(x => x.name == "GameplayMenu").transform
				.Find("MenuGameplayBasic")
				.Find("UIElement-ControllerProfile")
				.Find("HorizontalLayoutGroup")
				.Find("LabelBlock")
				.Find("LineBreak_Dots")
				.GetComponent<Image>()
				.sprite;

			var imageObj = new GameObject("dots");
			imageObj.transform.parent = separatorObj.transform;
			imageObj.transform.localPosition = Vector3.zero;
			imageObj.transform.localScale = Vector3.one;

			var rt = imageObj.GetAddComponent<RectTransform>();
			rt.anchorMin = new Vector2(0, 0.5f);
			rt.anchorMax = new Vector2(1, 0.5f);
			rt.pivot = new Vector2(0.5f, 0.5f);
			rt.sizeDelta = new Vector2(0, 5);
			rt.offsetMin = new Vector2(0, rt.offsetMin.y);
			rt.offsetMax = new Vector2(0, rt.offsetMax.y);
			rt.anchoredPosition = new Vector2(0, 0);
			rt.localScale = Vector3.one;

			var image = imageObj.AddComponent<Image>();
			image.sprite = dotsSprite;
			image.color = new Color(0.2196079f, 0.2196079f, 0.2196079f);
			image.type = Image.Type.Tiled;
			image.preserveAspect = true;

			return separatorObj;
		}

		public SubmitAction CreateButton(Menu menu, string buttonLabel, string tooltip, MenuSide side)
		{
			var existingButton = Resources.FindObjectsOfTypeAll<Menu>()
				.Single(x => x.name == "GraphicsMenu").transform
				.Find("Scroll View")
				.Find("Viewport")
				.Find("Content")
				.Find("GammaButtonPanel").gameObject;

			var newButtonObj = Object.Instantiate(existingButton);
			newButtonObj.transform.parent = GetParentForAddedElements(menu);
			newButtonObj.transform.localPosition = Vector3.zero;
			newButtonObj.transform.localScale = Vector3.one;
			newButtonObj.name = $"UIElement-Button-{buttonLabel}";
			newButtonObj.transform.localRotation = Quaternion.identity;

			// the thing we're copying is already LEFT, so dont need to handle it
			if (side == MenuSide.CENTER)
			{
				Object.Destroy(newButtonObj.transform.Find("RightSpacer").gameObject);
			}
			else if (side == MenuSide.RIGHT)
			{
				newButtonObj.transform.Find("RightSpacer").SetAsFirstSibling();
			}

			var uielement = newButtonObj.transform.Find("UIElement-GammaButton").gameObject;

			Object.Destroy(uielement.GetComponent<SubmitActionMenu>());
			var submitAction = uielement.AddComponent<SubmitAction>();

			Object.Destroy(uielement.GetComponentInChildren<LocalizedText>());

			var menuOption = uielement.GetComponent<MenuOption>();
			menuOption._tooltipTextType = UITextType.None;
			menuOption._overrideTooltipText = tooltip;
			menuOption._label.text = buttonLabel;

			menu._menuOptions = menu._menuOptions.Add(menuOption);

			if (menu._selectOnActivate == null)
			{
				menu._selectOnActivate = newButtonObj.GetComponent<Selectable>();
			}

			uielement.AddComponent<SelectableAudioPlayer>();

			return submitAction;
		}

		public SubmitAction CreateButtonWithLabel(Menu menu, string label, string buttonLabel, string tooltip)
		{
			var newButtonObj = new GameObject($"UIElement-{label}");
			newButtonObj.transform.parent = GetParentForAddedElements(menu);
			newButtonObj.transform.localScale = Vector3.one;

			var layoutElement = newButtonObj.AddComponent<LayoutElement>();
			layoutElement.minHeight = 70;
			layoutElement.flexibleWidth = 1;

			var existingHorizLayout = Resources.FindObjectsOfTypeAll<Menu>()
				.Single(x => x.name == "GraphicsMenu").transform
				.Find("Scroll View")
				.Find("Viewport")
				.Find("Content")
				.Find("UIElement-ResolutionSelect")
				.Find("HorizontalLayoutGroup").gameObject;

			var newHorizLayout = Object.Instantiate(existingHorizLayout);
			newHorizLayout.transform.parent = newButtonObj.transform;
			newHorizLayout.transform.localPosition = Vector3.zero;
			newHorizLayout.transform.localScale = Vector3.one;
			newHorizLayout.transform.localRotation = Quaternion.identity;

			var hrt = newHorizLayout.GetComponent<RectTransform>();
			var ohrt = existingHorizLayout.GetComponent<RectTransform>();
			hrt.anchorMin = ohrt.anchorMin;
			hrt.anchorMax = ohrt.anchorMax;
			hrt.offsetMin = ohrt.offsetMin;
			hrt.offsetMax = ohrt.offsetMax;
			hrt.anchoredPosition3D = ohrt.anchoredPosition3D;
			hrt.sizeDelta = ohrt.sizeDelta;

			Object.Destroy(newHorizLayout.GetComponentInChildren<LocalizedText>());

			var labelComponent = newHorizLayout.transform
				.Find("LabelBlock")
				.Find("HorizontalLayoutGroup")
				.Find("Label")
				.GetComponent<Text>();
			labelComponent.text = label;

			var controlBlock = newHorizLayout.transform.Find("ControlBlock");
			Object.Destroy(controlBlock.Find("OptionSelectorBG").gameObject);
			Object.Destroy(controlBlock.Find("HorizontalLayoutGroup").gameObject);

			var existingButton = Resources.FindObjectsOfTypeAll<Menu>()
				.Single(x => x.name == "GraphicsMenu").transform
				.Find("Scroll View")
				.Find("Viewport")
				.Find("Content")
				.Find("GammaButtonPanel")
				.Find("UIElement-GammaButton").gameObject;

			var newButton = Object.Instantiate(existingButton);
			newButton.transform.parent = controlBlock;
			newButton.transform.localScale = Vector3.one;
			newButton.transform.localPosition = Vector3.zero;
			newButton.name = $"UIElement-{label}";
			newButton.transform.localRotation = Quaternion.identity;

			var rt = newButton.GetComponent<RectTransform>();
			var ort = existingHorizLayout.transform.Find("ControlBlock").Find("HorizontalLayoutGroup").GetComponent<RectTransform>();
			rt.anchorMin = ort.anchorMin;
			rt.anchorMax = ort.anchorMax;
			rt.offsetMin = ort.offsetMin;
			rt.offsetMax = ort.offsetMax;
			rt.anchoredPosition3D = ort.anchoredPosition3D;
			rt.sizeDelta = ort.sizeDelta;

			Object.Destroy(newButton.GetComponent<SubmitActionMenu>());
			var submitAction = newButton.AddComponent<SubmitAction>();

			Object.Destroy(newButton.GetComponentInChildren<LocalizedText>());

			var leftArrow = newHorizLayout.transform
				.Find("LabelBlock")
				.Find("HorizontalLayoutGroup")
				.Find("LeftArrow").GetComponent<Image>();

			var rightArrow = newHorizLayout.transform
				.Find("LabelBlock")
				.Find("HorizontalLayoutGroup")
				.Find("RightArrow").GetComponent<Image>();

			var uiStyleApplier = newButtonObj.AddComponent<UIStyleApplier>();
			uiStyleApplier._textItems = new Text[] { labelComponent };
			uiStyleApplier._foregroundGraphics = new Graphic[] { labelComponent };
			uiStyleApplier._backgroundGraphics = new Graphic[] { };
			uiStyleApplier._onOffGraphicList = new UIStyleApplier.OnOffGraphic[]
			{
				new UIStyleApplier.OnOffGraphic()
				{
					graphic = leftArrow,
					visibleNormal = false,
					visibleIntermediate = false,
					visibleHighlighted = true,
					visiblePressed = true,
					visibleDisabled = false,
					visibleMouseRollover = true
				},
				new UIStyleApplier.OnOffGraphic()
				{
					graphic = rightArrow,
					visibleNormal = false,
					visibleIntermediate = false,
					visibleHighlighted = true,
					visiblePressed = true,
					visibleDisabled = false,
					visibleMouseRollover = true
				}
			};

			newButton.GetAddComponent<SelectableAudioPlayer>();

			var menuOption = newButton.GetComponent<MenuOption>();
			menuOption._tooltipTextType = UITextType.None;
			menuOption._overrideTooltipText = tooltip;
			menuOption._label.text = buttonLabel;

			menu._menuOptions = menu._menuOptions.Add(menuOption);

			if (menu._selectOnActivate == null)
			{
				menu._selectOnActivate = newButtonObj.GetComponent<Selectable>();
			}

			return submitAction;
		}

		public IOWMLTextEntryElement AddTextEntryInput(Menu menu, string label, string initialValue, string tooltip, bool isNumeric)
		{
			var submitAction = CreateButtonWithLabel(menu, label, initialValue, tooltip);
			var textInputPopup = _popupMenuManager.CreateInputFieldPopup($"Enter the new value for \"{label}\".", initialValue, "Confirm", "Cancel");
			submitAction.OnSubmitAction += () => textInputPopup.EnableMenu(true);

			var textEntry = submitAction.gameObject.AddComponent<OWMLTextEntryElement>();
			textEntry._overrideTooltipText = tooltip;
			textEntry.RegisterPopup(textInputPopup);
			textEntry.IsNumeric = isNumeric;

			if (isNumeric)
			{
				textInputPopup.OnInputPopupValidateChar += c =>
				{
					var text = textInputPopup.GetInputText() + c;
					return Regex.IsMatch(text, @"^\d*[,.]?\d*$");
				};
			}

			return textEntry;
		}

		public void CreateLabel(Menu menu, string label)
		{
			var newObj = new GameObject("Label");
			
			var layoutElement = newObj.AddComponent<LayoutElement>();
			layoutElement.flexibleWidth = 1;

			var verticalLayout = newObj.AddComponent<VerticalLayoutGroup>();
			verticalLayout.padding = new RectOffset(100, 100, 0, 0);
			verticalLayout.spacing = 0;
			verticalLayout.childAlignment = TextAnchor.MiddleCenter;
			verticalLayout.childForceExpandHeight = false;
			verticalLayout.childForceExpandWidth = false;
			verticalLayout.childControlHeight = true;
			verticalLayout.childControlWidth = true;
			verticalLayout.childScaleHeight = false;
			verticalLayout.childScaleWidth = false;

			var textObj = new GameObject("Text");
			
			var text = textObj.AddComponent<Text>();
			text.text = label;
			text.font = Resources.Load<Font>("fonts/english - latin/Adobe - SerifGothicStd");
			text.fontSize = 36;
			text.alignment = TextAnchor.MiddleCenter;
			text.horizontalOverflow = HorizontalWrapMode.Wrap;
			text.verticalOverflow = VerticalWrapMode.Truncate;

			var textLayoutElement = textObj.AddComponent<LayoutElement>();
			textLayoutElement.minHeight = 70;

			textObj.transform.parent = newObj.transform;
			textObj.transform.localScale = Vector3.one;
			textObj.transform.localPosition = Vector3.zero;
			textObj.transform.localRotation = Quaternion.identity;

			newObj.transform.parent = GetParentForAddedElements(menu);
			newObj.transform.localScale = Vector3.one;
			newObj.transform.localPosition = Vector3.zero;
			newObj.transform.localRotation = Quaternion.identity;
		}

		private TabButton CreateTabButton(string name, Menu menu)
		{
			var existingButton = Resources.FindObjectsOfTypeAll<TabButton>().Single(x => x.name == "Button-Graphics");

			var newButton = Object.Instantiate(existingButton);
			newButton.transform.parent = existingButton.transform.parent;
			newButton.transform.localScale = Vector3.one;
			newButton.transform.localPosition = Vector3.zero;
			newButton.transform.SetSiblingIndex(newButton.transform.parent.childCount - 2);
			newButton.name = $"Button-{name}";
			newButton.transform.localRotation = Quaternion.identity;

			RecalculateNavigation(newButton.transform.parent.GetComponentsInChildren<Button>(true).ToList());

			var text = newButton.GetComponentInChildren<Text>();
			Object.Destroy(text.GetComponent<LocalizedText>());
			text.text = name;
			text.horizontalOverflow = HorizontalWrapMode.Wrap;

			var tabButton = newButton.GetComponent<TabButton>();
			tabButton._tabbedMenu = menu ?? throw new System.Exception("Menu cannot be null.");

			return tabButton;
		}

		private void RecalculateNavigation(List<Button> tabButtons)
		{
			if (tabButtons.Count == 1)
			{
				SetSelectOnLeft(tabButtons[0], tabButtons[0]);
				SetSelectOnRight(tabButtons[0], tabButtons[0]);
				return;
			}

			// deal with edge rollover
			SetSelectOnLeft(tabButtons[0], tabButtons.Last());
			SetSelectOnRight(tabButtons.Last(), tabButtons[0]);

			for (var i = 0; i < tabButtons.Count; i++)
			{
				if (i == 0)
				{
					SetSelectOnRight(tabButtons[i], tabButtons[i + 1]);
				}
				else if (i == tabButtons.Count - 1)
				{
					SetSelectOnLeft(tabButtons[i], tabButtons[i - 1]);
				}
				else
				{
					SetSelectOnLeft(tabButtons[i], tabButtons[i - 1]);
					SetSelectOnRight(tabButtons[i], tabButtons[i + 1]);
				}
			}
		}

		private void SetSelectOnLeft(Button button, Selectable selectable)
		{
			var navigation = button.navigation;
			navigation.selectOnLeft = selectable;
			button.navigation = navigation;
		}

		private void SetSelectOnRight(Button button, Selectable selectable)
		{
			var navigation = button.navigation;
			navigation.selectOnRight = selectable;
			button.navigation = navigation;
		}

		private Transform GetParentForAddedElements(Menu menu)
		{
			if (menu.transform.Find("Scroll View") != null)
			{
				return menu.transform.Find("Scroll View").Find("Viewport").Find("Content");
			}

			if (menu.transform.Find("Content") != null)
			{
				return menu.transform.Find("Content");
			}

			return menu.transform;
		}
	}
}
