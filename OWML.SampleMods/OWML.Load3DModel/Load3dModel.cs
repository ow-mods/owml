using OWML.Common;
using UnityEngine;

namespace OWML.Load3DModel
{
    public class Load3DModel : ModBehaviour
    {
        private bool _isStarted;
        private GameObject _duck;

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
                _isStarted = true;
            }
        }

        private void Update()
        {
            if (_isStarted && Input.GetMouseButtonDown(0))
            {
                ModHelper.Console.WriteLine("Creating duck...");
                var duck = Instantiate(_duck, GameObject.FindWithTag("Player").transform.position, Quaternion.identity);
                ModHelper.Console.WriteLine("Created duck!");
            }
        }

    }
}
