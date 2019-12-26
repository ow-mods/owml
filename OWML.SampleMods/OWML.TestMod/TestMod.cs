using OWML.Common;
using OWML.ModHelper;
using UnityEngine;

namespace OWML.TestMod
{
    public class TestMod : ModBehaviour
    {

        private void Start()
        {
            ModHelper.Console.WriteLine($"In {nameof(TestMod)}!");
            ModHelper.Events.AddEvent<Flashlight>(Events.AfterStart);
            ModHelper.Events.OnEvent += OnEvent;
        }

        private void OnEvent(MonoBehaviour behaviour, Events ev)
        {
            ModHelper.Console.WriteLine("Behaviour name: " + behaviour.name);
            if (behaviour.GetType() == typeof(Flashlight) && ev == Events.AfterStart)
            {
                ModHelper.Console.WriteLine("BOOM!");
                GlobalMessenger.FireEvent("TriggerSupernova");
            }
        }

    }
}
