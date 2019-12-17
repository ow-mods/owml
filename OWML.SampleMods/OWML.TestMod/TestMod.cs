using OWML.Common;
using UnityEngine;

namespace OWML.TestMod
{
    public class TestMod : ModBehaviour
    {

        private void Start()
        {
            ModHelper.Console.WriteLine($"In {nameof(TestMod)}!");
            ModHelper.Events.AddStartEvent<Flashlight>();
            ModHelper.Events.OnStart += OnStart;
        }

        private void OnStart(MonoBehaviour behaviour)
        {
            ModHelper.Console.WriteLine("Behaviour name: " + behaviour.name);
            if (behaviour.GetType() == typeof(Flashlight))
            {
                ModHelper.Console.WriteLine("BOOM!");
                GlobalMessenger.FireEvent("TriggerSupernova");
            }
        }

    }
}
