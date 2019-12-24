using OWML.Common;
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
            _duckBody = CreateDuck();
            _shootSound = ModHelper.Assets.LoadAudio(this, "blaster-firing.wav");
            _music = ModHelper.Assets.LoadAudio(this, "spiral-mountain.mp3");
            ModHelper.Events.AddEvent<Flashlight>(Common.Events.AfterStart);
            ModHelper.Events.OnEvent += OnEvent;
        }

        private OWRigidbody CreateDuck()
        {
            var duck = ModHelper.Assets.Load3DObject(this, "duck.obj", "duck.png");
            duck.AddComponent<SphereCollider>();
            duck.AddComponent<Rigidbody>();
            var duckBody = duck.AddComponent<OWRigidbody>();
            return duckBody;
        }

        private void OnEvent(MonoBehaviour behaviour, Common.Events ev)
        {
            if (behaviour.GetType() == typeof(Flashlight) && ev == Common.Events.AfterStart)
            {
                _playerTransform = Locator.GetPlayerTransform();
                _playerBody = _playerTransform.GetAttachedOWRigidbody();
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
            duckBody.SetPosition(_playerTransform.position + _playerTransform.forward * 1f);
            duckBody.SetRotation(_playerTransform.rotation);
            duckBody.SetVelocity(_playerBody.GetVelocity() + _playerTransform.forward * 10f);
            _shootSound.Play();
        }
    }
}
