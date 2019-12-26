using OWML.Common;
using OWML.ModHelper;
using UnityEngine;

namespace OWML.LoadCustomAssets
{
    public class LoadCustomAssets : ModBehaviour
    {
        private bool _isStarted;
        private OWRigidbody _duckBody;
        private Transform _playerTransform;
        private OWRigidbody _playerBody;
        private AudioSource _shootSound;
        private AudioSource _music;

        private void Start()
        {
            ModHelper.Console.WriteLine($"In {nameof(LoadCustomAssets)}!");
            var gunSoundAsset = ModHelper.Assets.LoadAudio(this, "blaster-firing.wav");
            gunSoundAsset.OnLoaded += OnGunSoundLoaded;
            var duckAsset = ModHelper.Assets.Load3DObject(this, "duck.obj", "duck.png");
            duckAsset.OnLoaded += OnDuckLoaded;
            var musicAsset = ModHelper.Assets.LoadAudio(this, "spiral-mountain.mp3");
            musicAsset.OnLoaded += OnMusicLoaded;
            ModHelper.Events.AddEvent<PlayerBody>(Events.AfterAwake);
            ModHelper.Events.OnEvent += OnEvent;
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
        }

        private void OnEvent(MonoBehaviour behaviour, Events ev)
        {
            if (behaviour.GetType() == typeof(PlayerBody) && ev == Events.AfterAwake)
            {
                _playerBody = (PlayerBody)behaviour;
                _playerTransform = behaviour.transform;
                _isStarted = true;
                _music.Play();
            }
        }

        private void Update()
        {
            if (_isStarted && Input.GetMouseButtonDown(0))
            {
                ShootDuck();
            }
        }

        private void ShootDuck()
        {
            ModHelper.Console.WriteLine("Shooting duck");
            var duckBody = Instantiate(_duckBody);
            duckBody.SetPosition(_playerTransform.position + _playerTransform.forward * 2f);
            duckBody.SetRotation(_playerTransform.rotation);
            duckBody.SetVelocity(_playerBody.GetVelocity() + _playerTransform.forward * 10f);
            _shootSound.Play();
        }
    }
}
