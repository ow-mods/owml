using System.Reflection;
using OWML.Common;
using UnityEngine;

namespace OWML.Load3DModel
{
    public class Load3DModel : ModBehaviour
    {
        private bool _isStarted;
        private GameObject _duck;
        private PlayerBody _player;

        private void Start()
        {
            ModHelper.Console.WriteLine($"In {nameof(Load3DModel)}!");
            _duck = ModHelper.Assets.Create3DObject(this, "duck.obj", "duck.png");
            ModHelper.Events.AddEvent<Flashlight>(Events.AfterStart);
            ModHelper.Events.OnEvent += OnEvent;
        }

        private void OnEvent(MonoBehaviour behaviour, Events ev)
        {
            if (behaviour.GetType() == typeof(Flashlight) && ev == Events.AfterStart)
            {
                _player = GameObject.FindObjectOfType<PlayerBody>();
                _isStarted = true;
            }
        }

        private void Update()
        {
            if (_isStarted && Input.GetMouseButtonDown(0))
            {
                ModHelper.Console.WriteLine("Creating duck");
                var pos = _player.transform.position + _player.transform.forward;
                var duck = Instantiate(_duck, pos, Quaternion.identity);
                var duckBody = duck.GetComponent<OWRigidbody>();
                duckBody.SetMass(_player.GetMass());
                duckBody.SetVelocity(_player.GetVelocity());
                duckBody.SetAngularVelocity(_player.GetAngularVelocity());
                duckBody.SetCenterOfMass(_player.GetCenterOfMass());
                duckBody.RegisterAttachedGravityVolume(_player.GetAttachedGravityVolume());
                duckBody.RegisterAttachedForceDetector(_player.GetAttachedForceDetector());
            }
        }

    }
}
