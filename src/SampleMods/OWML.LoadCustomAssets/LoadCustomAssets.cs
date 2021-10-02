using System.Collections.Generic;
using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper;
using UnityEngine.InputSystem;
using UnityEngine;

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

			var modMenu = ModHelper.Menus.ModsMenu.GetModMenu(this);

			TestLogging();

			TestPopup();
		}

		private void TestPopup()
		{
			ModHelper.Menus.PauseMenu.OnInit += () =>
			{
				var popupButton = ModHelper.Menus.PauseMenu.ResumeButton.Duplicate("POPUP TEST");
				popupButton.OnClick += () =>
				{
					ModHelper.Console.WriteLine("making popup, hopefully");
					var popup = ModHelper.Menus.PopupManager.CreateInputPopup(InputType.Text, "Event Name");
					popup.OnConfirm += s => ModHelper.Console.WriteLine("clicked confirm");
				};
			};
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
			bool ready = true;
			if (Mouse.current == null)
			{
				ModHelper.Console.WriteLine("Mouse object is null!", MessageType.Error);
				ready = false;
			}
			if (Keyboard.current == null)
			{
				ModHelper.Console.WriteLine("Keyboard object is null!", MessageType.Error);
				ready = false;
			}
			_isInSolarSystem = newScene == OWScene.SolarSystem && ready;
			ToggleMusic(ModHelper.Config.GetSettingsValue<bool>("enableMusic"));
		}

		public void Update()
		{
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
