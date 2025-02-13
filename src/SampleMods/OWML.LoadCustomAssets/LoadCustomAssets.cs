using System.Collections.Generic;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine.InputSystem;
using UnityEngine;
using OWML.Common.Interfaces.Menus;

namespace OWML.LoadCustomAssets
{
	public class LoadCustomAssets : ModBehaviour
	{
		private enum ABC
		{
			A,
			B,
			C
		}

		private OWRigidbody _duckBody;
		private Transform _playerTransform;
		private OWRigidbody _playerBody;
		private AudioSource _shootSound;
		private AudioSource _music;
		private SaveFile _saveFile;
		private GameObject _cube;

		private bool _isInSolarSystem;
		private bool _isDucksEnabled;
		private bool _isCubesEnabled;

		private readonly List<GameObject> _ducks = new();

		public void Start()
		{
			ModHelper.Console.WriteLine($"In {nameof(LoadCustomAssets)}!", MessageType.Info);
			_saveFile = ModHelper.Storage.Load<SaveFile>("savefile.json");
			ModHelper.Console.WriteLine("Ducks shot: " + _saveFile.NumberOfDucks);

			var assetBundle = ModHelper.Assets.LoadBundle("cubebundle");
			_cube = assetBundle.LoadAsset<GameObject>("Cube");

			ModHelper.Events.Player.OnPlayerAwake += OnPlayerAwake;
			ModHelper.Events.Scenes.OnStartSceneChange += OnStartSceneChange;
			ModHelper.Events.Scenes.OnCompleteSceneChange += OnCompleteSceneChange;

			TestLogging();

			TestAPI();

			ModHelper.MenuHelper.PopupMenuManager.RegisterStartupPopup("Test Startup Popup");
		}

		public IOWMLFourChoicePopupMenu FourChoicePopupMenu;
		public IOWMLPopupInputMenu PopupInput;

		public override void SetupTitleMenu(ITitleMenuManager titleManager)
		{
			var infoButton = titleManager.CreateTitleButton("INFO POPUP");
			var infoPopup = ModHelper.MenuHelper.PopupMenuManager.CreateInfoPopup("test info popup", "yarp");
			infoButton.OnSubmitAction += () => infoPopup.EnableMenu(true);
			infoPopup.OnActivateMenu += () => ModHelper.Console.WriteLine("info popup activate");
			infoPopup.OnDeactivateMenu += () => ModHelper.Console.WriteLine("info popup deactivate");

			var twoChoiceButton = titleManager.CreateTitleButton("TWO CHOICE");
			var twoChoicePopup = ModHelper.MenuHelper.PopupMenuManager.CreateTwoChoicePopup("test two choice popup", "oak", "narp");
			twoChoiceButton.OnSubmitAction += () => twoChoicePopup.EnableMenu(true);
			twoChoicePopup.OnActivateMenu += () => ModHelper.Console.WriteLine("two popup activate");
			twoChoicePopup.OnDeactivateMenu += () => ModHelper.Console.WriteLine("two popup deactivate");

			var threeChoiceButton = titleManager.CreateTitleButton("THREE CHOICE");
			var threeChoicePopup = ModHelper.MenuHelper.PopupMenuManager.CreateThreeChoicePopup("test three choice popup", "oak", "oak (better)", "narp");
			threeChoiceButton.OnSubmitAction += () => threeChoicePopup.EnableMenu(true);
			threeChoicePopup.OnPopupConfirm1 += () => ModHelper.Console.WriteLine("Confirm 1");
			threeChoicePopup.OnPopupConfirm2 += () => ModHelper.Console.WriteLine("Confirm 2");
			threeChoicePopup.OnActivateMenu += () => ModHelper.Console.WriteLine("three popup activate");
			threeChoicePopup.OnDeactivateMenu += () => ModHelper.Console.WriteLine("three popup deactivate");

			var fourChoiceButton = titleManager.CreateTitleButton("FOUR CHOICE");
			FourChoicePopupMenu = ModHelper.MenuHelper.PopupMenuManager.CreateFourChoicePopup("test four choice popup", "oak", "oak (better)", "oak (worse)", "narp");
			fourChoiceButton.OnSubmitAction += () => FourChoicePopupMenu.EnableMenu(true);
			FourChoicePopupMenu.OnPopupConfirm1 += () => ModHelper.Console.WriteLine("Confirm 1");
			FourChoicePopupMenu.OnPopupConfirm2 += () => ModHelper.Console.WriteLine("Confirm 2");
			FourChoicePopupMenu.OnPopupConfirm3 += () => ModHelper.Console.WriteLine("Confirm 3");
			FourChoicePopupMenu.OnActivateMenu += () => ModHelper.Console.WriteLine("four popup activate");
			FourChoicePopupMenu.OnDeactivateMenu += () => ModHelper.Console.WriteLine("four popup deactivate");

			var textButton = titleManager.CreateTitleButton("INPUT POPUP TEST");
			PopupInput = ModHelper.MenuHelper.PopupMenuManager.CreateInputFieldPopup("test text popup", "type a funny thing!", "ok", "cancel");
			textButton.OnSubmitAction += () => PopupInput.EnableMenu(true);
			PopupInput.OnPopupConfirm += () =>
			{
				ModHelper.Console.WriteLine(PopupInput.GetInputText());
			};

			PopupInput.OnActivateMenu += () => ModHelper.Console.WriteLine("text popup activate");
			PopupInput.OnDeactivateMenu += () => ModHelper.Console.WriteLine("text popup deactivate");
		}

		public override void CleanupTitleMenu()
		{
			ModHelper.Console.WriteLine($"CLEANUP TITLE MENU");
		}

		public override void SetupPauseMenu(IPauseMenuManager pauseManager)
		{
			var listMenu = pauseManager.MakePauseListMenu("TEST");
			var button = pauseManager.MakeMenuOpenButton("TEST", listMenu, 1, true);

			var button1 = pauseManager.MakeSimpleButton("1", 0, true, listMenu);
			var button2 = pauseManager.MakeSimpleButton("2", 1, true, listMenu);
			var button3 = pauseManager.MakeSimpleButton("3", 2, true, listMenu);

			pauseManager.PauseMenuOpened += LogOpened;
			pauseManager.PauseMenuClosed += LogClosed;
		}

		public override void CleanupPauseMenu()
		{
			ModHelper.MenuHelper.PauseMenuManager.PauseMenuOpened -= LogOpened;
			ModHelper.MenuHelper.PauseMenuManager.PauseMenuClosed -= LogClosed;
		}

		private void LogOpened()
		{
			ModHelper.Console.WriteLine($"PAUSE MENU OPENED!", MessageType.Success);
		}

		private void LogClosed()
		{
			ModHelper.Console.WriteLine($"PAUSE MENU CLOSED!", MessageType.Success);
		}

		public override void SetupOptionsMenu(IOptionsMenuManager optionsManager)
		{
			var infoPopup = ModHelper.MenuHelper.PopupMenuManager.CreateInfoPopup("test info popup", "yarp");
			var twoChoicePopup = ModHelper.MenuHelper.PopupMenuManager.CreateTwoChoicePopup("test two choice popup", "oak", "narp");
			var threeChoicePopup = ModHelper.MenuHelper.PopupMenuManager.CreateThreeChoicePopup("test three choice popup", "oak", "oak (better)", "narp");

			var (tabMenu, tabButton) = optionsManager.CreateTabWithSubTabs("TEST");
			var (subTab1Menu, subTab1Button) = optionsManager.AddSubTab(tabMenu, "TAB 1");
			var (subTab2Menu, subTab2Button) = optionsManager.AddSubTab(tabMenu, "TAB 2");

			var infoPopupButton = optionsManager.CreateButton(subTab1Menu, "Info Popup", "Opens an info popup.", MenuSide.LEFT);
			infoPopupButton.OnSubmitAction += () => infoPopup.EnableMenu(true);
			var twoButton = optionsManager.CreateButton(subTab1Menu, "Two Choice Popup", "Opens a two choice popup.", MenuSide.CENTER);
			twoButton.OnSubmitAction += () => twoChoicePopup.EnableMenu(true);
			var threeButton = optionsManager.CreateButton(subTab1Menu, "Three Choice Popup", "Opens a three choice popup.", MenuSide.RIGHT);
			threeButton.OnSubmitAction += () => threeChoicePopup.EnableMenu(true);

			var checkbox = optionsManager.AddCheckboxInput(subTab2Menu, "Test Checkbox", "* It's a test checkbox.", false);
			var toggle = optionsManager.AddToggleInput(subTab2Menu, "Test Toggle", "Option 1", "Option 2", "* It's a test toggle.", false);
			var selector = optionsManager.AddSelectorInput(subTab2Menu, "Test Selector", new[] { "Option 1", "Option 2", "Option 3" }, "* It's a test selector.", true, 0);
			var slider = optionsManager.AddSliderInput(subTab2Menu, "Test Slider", 0, 100, "* It's a test slider.", 50);
		}

		public override void CleanupOptionsMenu()
		{
			ModHelper.Console.WriteLine($"CLEANUP OPTIONS MENU");
		}

		private void TestAPI()
		{
			var api = ModHelper.Interaction.TryGetModApi<IAPI>("_nebula.ExampleAPI");
			ModHelper.Console.WriteLine(api.Echo("Test API echo!"));
			ModHelper.Console.WriteLine("Test API radio: " + api.Radio<ABC>().ToString());
		}

		private void TestPopup()
		{
			/*ModHelper.Menus.PauseMenu.OnInit += () =>
			{
				var popupButton = ModHelper.Menus.PauseMenu.ResumeButton.Duplicate("POPUP TEST");
				popupButton.OnClick += () =>
				{
					ModHelper.Console.WriteLine("making popup, hopefully");
					var popup = ModHelper.Menus.PopupManager.CreateInputPopup(InputType.Text, "Event Name");
					popup.OnConfirm += s => ModHelper.Console.WriteLine("clicked confirm");
				};
			};*/
		}

		public override void Configure(IModConfig config)
		{
			ToggleMusic(config.GetSettingsValue<bool>("enableMusic"));
			_isDucksEnabled = config.GetSettingsValue<bool>("enableDucks");
			_isCubesEnabled = config.GetSettingsValue<bool>("enableCubes");
			var speed = config.GetSettingsValue<float>("speed");
			var power = config.GetSettingsValue<float>("power");
			var enableSuperMode = config.GetSettingsValue<bool>("enableSuperMode");
			var selectedEnum = config.GetSettingsValue<ABC>("thing");
			var selectedString = config.GetSettingsValue<string>("thing");
			var selectedInt = config.GetSettingsValue<int>("integer thing");
			ModHelper.Console.WriteLine($"Selected enum = {selectedEnum}, string = {selectedString}");
		}

		public void TestLogging()
		{
			ModHelper.Console.WriteLine("Test Error", MessageType.Error);
			ModHelper.Console.WriteLine("Test Warning", MessageType.Warning);
			ModHelper.Console.WriteLine("Test Message", MessageType.Message);
			ModHelper.Console.WriteLine("Test Success", MessageType.Success);
			ModHelper.Console.WriteLine("Test Info", MessageType.Info);
			ModHelper.Console.WriteLine("Test Debug", MessageType.Debug);
		}

		private void LoadMusic()
		{
			_music = new GameObject().AddComponent<AudioSource>();
			_music.clip = ModHelper.Assets.GetAudio("spiral-mountain.mp3");
			ModHelper.Console.WriteLine("Music loaded!");
		}

		private void LoadGunSound()
		{
			_shootSound = new GameObject().AddComponent<AudioSource>();
			_shootSound.clip = ModHelper.Assets.GetAudio("blaster-firing.wav");
			ModHelper.Console.WriteLine("Gun sound loaded!");
		}

		private void LoadDuck()
		{
			var duck = ModHelper.Assets.Get3DObject("duck.obj", "duck.png");
			ModHelper.Console.WriteLine("Duck loaded!");

			duck.AddComponent<SphereCollider>();
			duck.AddComponent<Rigidbody>();
			_duckBody = duck.AddComponent<OWRigidbody>();
			duck.SetActive(false);
		}

		private void OnPlayerAwake(PlayerBody playerBody)
		{
			_playerBody = playerBody;
			_playerTransform = playerBody.transform;
			ModHelper.Console.WriteLine("Player loaded!");

			LoadDuck();
			LoadGunSound();
			LoadMusic();
		}

		private void OnStartSceneChange(OWScene oldScene, OWScene newScene)
		{
			if (oldScene == OWScene.SolarSystem)
			{
				_ducks.ForEach(Destroy);
			}
		}

		private void OnCompleteSceneChange(OWScene oldScene, OWScene newScene)
		{
			_isInSolarSystem = newScene == OWScene.SolarSystem;
			ToggleMusic(ModHelper.Config.GetSettingsValue<bool>("enableMusic"));
		}

		public void Update()
		{
			if (FourChoicePopupMenu != null)
			{
				var rnd = new System.Random();
				FourChoicePopupMenu.SetText("blah", rnd.Next().ToString(), rnd.Next().ToString(), rnd.Next().ToString(), rnd.Next().ToString());
			}

			if (PopupInput != null)
			{
				var rnd = new System.Random();
				PopupInput.SetText("blah", rnd.Next().ToString(), rnd.Next().ToString(), rnd.Next().ToString());
			}

			if (Keyboard.current[Key.F9].wasPressedThisFrame)
			{
				SendFatalMessage();
			}
			if (!_isInSolarSystem || OWTime.IsPaused())
			{
				return;
			}
			if (Mouse.current.leftButton.wasPressedThisFrame && _isDucksEnabled)
			{
				ShootDuck();
				return;
			}
			if (Mouse.current.rightButton.wasPressedThisFrame && _isCubesEnabled)
			{
				CreateCube();
			}
		}

		private void SendFatalMessage()
		{
			ModHelper.Console.WriteLine("I have chosen to crash the game", MessageType.Fatal);
		}

		private void ShootDuck()
		{
			var duckBody = Instantiate(_duckBody);
			duckBody.gameObject.SetActive(true);

			duckBody.SetPosition(_playerTransform.position + _playerTransform.forward * 2f);
			duckBody.SetRotation(_playerTransform.rotation);
			duckBody.SetVelocity(_playerBody.GetVelocity() + _playerTransform.forward * 10f);
			_ducks.Add(duckBody.gameObject);

			_shootSound.Play();

			_saveFile.NumberOfDucks++;
			ModHelper.Console.WriteLine($"Ducks shot: {_saveFile.NumberOfDucks}");
			ModHelper.Storage.Save(_saveFile, "savefile.json");
		}

		private void CreateCube()
		{
			Instantiate(_cube, _playerTransform.position + _playerTransform.forward * 2f, Quaternion.identity);
		}

		private void ToggleMusic(bool enable)
		{
			ModHelper.Console.WriteLine($"ToggleMusic: {enable}");
			if (_music == null)
			{
				return;
			}
			if (enable && _isInSolarSystem)
			{
				_music.Play();
			}
			else if (!enable || !_isInSolarSystem)
			{
				_music.Stop();
			}
		}
	}
}
