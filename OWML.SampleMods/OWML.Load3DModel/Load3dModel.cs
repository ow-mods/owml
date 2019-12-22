using OWML.Common;
using UnityEngine;

namespace OWML.Load3DModel
{
    public class Load3DModel : ModBehaviour
    {

        private void Start()
        {
            ModHelper.Console.WriteLine($"In {nameof(Load3DModel)}!");
            ModHelper.Events.AddEvent<Flashlight>(Events.AfterStart);
            ModHelper.Events.OnEvent += OnEvent;
        }

        private bool _isStarted;

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
                var duck = ModHelper.Assets.Create3DObject(this, "duck.obj", "duck.png");
                ModHelper.Console.WriteLine("Created duck!");
            }
        }

    }
}
