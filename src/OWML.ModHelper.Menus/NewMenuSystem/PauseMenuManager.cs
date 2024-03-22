using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OWML.Common;
using OWML.Utils;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace OWML.ModHelper.Menus.NewMenuSystem
{
	public class PauseMenuManager : IPauseMenuManager
	{
		private IModConsole _console;
		private FontAndLanguageController _languageController;
		private GameObject _pauseMenuItemsTemplate;
		private GameObject _buttonPrefab;

		public event Action PauseMenuOpened;
		public event Action PauseMenuClosed;

		public PauseMenuManager(IModConsole console)
		{
			_console = console;
			LoadManager.OnCompleteSceneLoad += OnSceneLoadCompleted;
		}

		private void OnSceneLoadCompleted(OWScene old, OWScene newScene)
		{
			if (newScene is OWScene.SolarSystem or OWScene.EyeOfTheUniverse)
			{
				var pauseMenuManager = Resources.FindObjectsOfTypeAll<global::PauseMenuManager>()[0];
				pauseMenuManager._pauseMenu.OnActivateMenu += () =>
				{
					if (!pauseMenuManager.IsOpen())
					{
						PauseMenuOpened?.SafeInvoke();
					}
				};

				pauseMenuManager._pauseMenu.OnDeactivateMenu += () =>
				{
					if (MenuStackManager.SharedInstance.GetMenuCount() == 0)
					{
						PauseMenuClosed?.SafeInvoke();
					}
				};
			}
		}

		private void AddToLangController(Text textComponent)
		{
			if (_languageController == null)
			{
				_languageController = Resources.FindObjectsOfTypeAll<global::PauseMenuManager>()[0].transform.GetChild(0).GetComponent<FontAndLanguageController>();
			}

			_languageController.AddTextElement(textComponent, false);
		}

		private void MakePauseMenuItemsTemplate()
		{
			var existingPauseMenuItems = GameObject.Find("PauseMenu/PauseMenuCanvas/PauseMenuBlock/PauseMenuItems");

			_pauseMenuItemsTemplate = Object.Instantiate(existingPauseMenuItems);
			Object.Destroy(_pauseMenuItemsTemplate.GetComponent<Menu>());

			var labelPausedText = _pauseMenuItemsTemplate.transform.Find("PauseMenuItemsLayout/LabelPaused/Text");
			Object.Destroy(labelPausedText.GetComponent<LocalizedText>());

			var unpause = _pauseMenuItemsTemplate.transform.GetChild(1).GetChild(2).gameObject;
			var options = _pauseMenuItemsTemplate.transform.GetChild(1).GetChild(3).gameObject;
			var endcurrentloop = _pauseMenuItemsTemplate.transform.GetChild(1).GetChild(4).gameObject;
			var exittomainmenu = _pauseMenuItemsTemplate.transform.GetChild(1).GetChild(5).gameObject;
			// using DestroyImmediate because the prefab is being edited in the same frame as it's instantiated
			Object.DestroyImmediate(unpause);
			Object.DestroyImmediate(options);
			Object.DestroyImmediate(endcurrentloop);
			Object.DestroyImmediate(exittomainmenu);

			_pauseMenuItemsTemplate.SetActive(false);

			var canvas = _pauseMenuItemsTemplate.GetComponent<Canvas>();
			if (canvas != null)
			{
				Object.Destroy(canvas);
			}

			var menu = _pauseMenuItemsTemplate.AddComponent<Menu>();
			menu._menuActivationRoot = _pauseMenuItemsTemplate;
		}

		public Menu MakePauseListMenu(string title)
		{
			if (_pauseMenuItemsTemplate == null)
			{
				MakePauseMenuItemsTemplate();
			}

			var newMenu = Object.Instantiate(_pauseMenuItemsTemplate);

			newMenu.transform.parent = GameObject.Find("PauseMenuBlock").transform;
			newMenu.transform.localScale = Vector3.one;
			newMenu.transform.localPosition = Vector3.zero;
			newMenu.transform.localRotation = Quaternion.identity;
			newMenu.GetComponent<RectTransform>().SetLeft(0);
			newMenu.GetComponent<RectTransform>().SetRight(0);
			newMenu.GetComponent<RectTransform>().SetTop(0);
			newMenu.GetComponent<RectTransform>().SetBottom(0);

			var text = newMenu.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>();
			text.text = title;
			newMenu.gameObject.name = title;

			return newMenu.GetComponent<Menu>();
		}

		public SubmitAction MakeSimpleButton(string name, int index, bool fromTop, Menu customMenu = null)
		{
			var button = CreateBase(name, index, fromTop, customMenu);
			button.SetActive(true);

			var submitAction = button.AddComponent<SubmitAction>();

			SetButtonText(submitAction, name);
			SetButtonIndex(submitAction, index, fromTop);
			AddToLangController(submitAction.GetComponentInChildren<Text>());

			return submitAction;
		}

		public SubmitAction MakeMenuOpenButton(string name, Menu menuToOpen, int index, bool fromTop, Menu customMenu = null)
		{
			if (LoadManager.GetCurrentScene() != OWScene.SolarSystem && LoadManager.GetCurrentScene() != OWScene.EyeOfTheUniverse)
			{
				_console.WriteLine("Error - Cannot create pause button in this scene!", OWML.Common.MessageType.Error);
				return null;
			}

			var menuRootObject = CreateBase(name, index, fromTop, customMenu);

			var submitActionMenu = menuRootObject.AddComponent<SubmitActionMenu>();
			submitActionMenu._menuToOpen = menuToOpen;

			SetButtonText(submitActionMenu, name);
			SetButtonIndex(submitActionMenu, index, fromTop);
			AddToLangController(submitActionMenu.GetComponentInChildren<Text>());

			menuRootObject.SetActive(true);
			return submitActionMenu;
		}

		public void SetButtonText(SubmitAction button, string text)
		{
			var textComp = button.GetComponentInChildren<Text>();
			textComp.text = text;
			textComp.SetAllDirty();
		}

		public void SetButtonIndex(SubmitAction button, int index, bool fromTop)
		{
			if (index > button.transform.parent.childCount - 4)
			{
				throw new IndexOutOfRangeException("Can't set a button to have a higher index than being last!");
			}

			var indexToSetTo = fromTop
				? index + 2
				: button.transform.parent.childCount - 2 - index;

			button.transform.SetSiblingIndex(indexToSetTo);
		}

		private GameObject CreateBase(string name, int index, bool fromTop, Menu customMenu = null)
		{
			if (_pauseMenuItemsTemplate == null)
			{
				// call this here so we can get a clean template before adding custom buttons
				MakePauseMenuItemsTemplate();
			}

			if (customMenu == null)
			{
				customMenu = Resources.FindObjectsOfTypeAll<Menu>().First(x => x.name == "PauseMenuItems");
			}

			if (_buttonPrefab == null)
			{
				var unpauseButton = GameObject.Find("PauseMenu/PauseMenuCanvas/PauseMenuBlock/PauseMenuItems/PauseMenuItemsLayout/Button-Unpause");
				_buttonPrefab = Object.Instantiate(unpauseButton);
				_buttonPrefab.SetActive(false);
				Object.Destroy(_buttonPrefab.GetComponent<Menu>());
				Object.Destroy(_buttonPrefab.GetComponent<SubmitActionCloseMenu>());
				var text = _buttonPrefab.transform.Find("HorizontalLayoutGroup/Text");
				Object.Destroy(text.GetComponent<LocalizedText>());
			}

			var pauseButton = Object.Instantiate(_buttonPrefab);

			// Make new button above dotted line
			var mainMenuLayoutGroup = customMenu.transform.GetChild(1).GetComponent<VerticalLayoutGroup>();
			pauseButton.transform.parent = mainMenuLayoutGroup.transform;
			pauseButton.transform.localPosition = Vector3.zero;
			pauseButton.transform.localScale = Vector3.one;
			pauseButton.transform.SetSiblingIndex(pauseButton.transform.GetSiblingIndex() - 1); // -1 because no spacer in pause menu
			pauseButton.SetActive(false);
			pauseButton.name = $"Button-{name}";
			pauseButton.transform.localRotation = Quaternion.identity;

			if (customMenu.GetSelectOnActivate() == null)
			{
				customMenu.SetSelectOnActivate(pauseButton.GetComponent<Button>());
			}

			return pauseButton;
		}
	}
}
