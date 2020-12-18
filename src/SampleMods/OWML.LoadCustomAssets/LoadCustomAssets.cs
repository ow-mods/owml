using System.Collections.Generic;
using OWML.Common;
using OWML.ModHelper;
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

		private readonly List<GameObject> _ducks = new List<GameObject>();

		private void Start()
		{
			ModHelper.Console.WriteLine($"In {nameof(LoadCustomAssets)}!", MessageType.Info);
			_saveFile = ModHelper.Storage.Load<SaveFile>("savefile.json");
			ModHelper.Console.WriteLine("Ducks shot: " + _saveFile.NumberOfDucks);

			var assetBundle = ModHelper.Assets.LoadBundle("cubebundle");
			_cube = assetBundle.LoadAsset<GameObject>("Cube");

			var gunSoundAsset = ModHelper.Assets.LoadAudio("blaster-firing.wav");
			gunSoundAsset.Loaded += OnGunSoundLoaded;
			var duckAsset = ModHelper.Assets.Load3DObject("duck.obj", "duck.png");
			duckAsset.Loaded += OnDuckLoaded;
			var musicAsset = ModHelper.Assets.LoadAudio("spiral-mountain.mp3");
			ModHelper.Events.Unity.RunWhen(() => musicAsset.Asset != null, () => OnMusicLoaded(musicAsset.Asset));

			ModHelper.Events.Player.OnPlayerAwake += OnPlayerAwake;
			ModHelper.Events.Scenes.OnStartSceneChange += OnStartSceneChange;
			ModHelper.Events.Scenes.OnCompleteSceneChange += OnCompleteSceneChange;

			var modMenu = ModHelper.Menus.ModsMenu.GetModMenu(this);

			TestLogging();
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
		}

		private void OnMusicLoaded(AudioSource audio)
		{
			_music = audio;
			ModHelper.Console.WriteLine("Music loaded!");
		}

		private void OnGunSoundLoaded(AudioSource audio)
		{
			_shootSound = audio;
			ModHelper.Console.WriteLine("Gun sound loaded!");
		}

		private void OnDuckLoaded(GameObject duck)
		{
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

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.F9))
			{
				SendFatalMessage();
			}
			if (!_isInSolarSystem || OWTime.IsPaused())
			{
				return;
			}
			if (Input.GetMouseButtonDown(0) && _isDucksEnabled)
			{
				ShootDuck();
			}
			else if (Input.GetMouseButtonDown(1) && _isCubesEnabled)
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
