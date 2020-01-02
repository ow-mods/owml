using OWML.Common;
using OWML.ModHelper.Events;
using OWML.ModHelper;
using UnityEngine;

namespace OWML.LightBramble
{
    public class LightBramble : ModBehaviour
    {
        private AudioSource _dekuSource;
        private bool _hasBrambleLoaded;

        private void Start()
        {
            ModHelper.Console.WriteLine("In " + nameof(LightBramble));

            ModHelper.Events.Subscribe<AnglerfishController>(Events.AfterEnable);
            ModHelper.Events.Subscribe<FogWarpVolume>(Events.AfterAwake);
            ModHelper.Events.Subscribe<PlanetaryFogController>(Events.AfterEnable);
            ModHelper.Events.Subscribe<FogOverrideVolume>(Events.AfterAwake);
            ModHelper.Events.Subscribe<GlobalMusicController>(Events.AfterStart);
            ModHelper.Events.OnEvent += OnEvent;

            var audioAsset = ModHelper.Assets.LoadAudio("deku-tree.mp3");
            audioAsset.OnLoaded += OnLoaded;
        }

        private void OnLoaded(AudioSource dekuSource)
        {
            ModHelper.Console.WriteLine("Deku Tree music loaded!");
            if (dekuSource.clip == null)
            {
                ModHelper.Console.WriteLine("Clip is null :(");
            }
            _dekuSource = dekuSource;
        }

        private void OnEvent(MonoBehaviour behaviour, Events ev)
        {
            if (behaviour.GetType() == typeof(AnglerfishController) && ev == Events.AfterEnable)
            {
                ModHelper.Console.WriteLine("Deactivating anglerfish");
                behaviour.gameObject.SetActive(false);
            }
            else if (behaviour.GetType().IsSubclassOf(typeof(FogWarpVolume)) && ev == Events.AfterAwake)
            {
                ModHelper.Console.WriteLine("Clearing _fogColor in FogWarpVolume");
                behaviour.SetValue("_fogColor", Color.clear);
            }
            else if (behaviour.GetType() == typeof(PlanetaryFogController) && ev == Events.AfterEnable)
            {
                ModHelper.Console.WriteLine("Clearing _fogTint in PlanetaryFogController");
                behaviour.SetValue("_fogTint", Color.clear);
            }
            else if (behaviour.GetType() == typeof(FogOverrideVolume) && ev == Events.AfterAwake)
            {
                ModHelper.Console.WriteLine("Clearing _tint in FogOverrideVolume");
                behaviour.SetValue("_tint", Color.clear);
            }
            else if (behaviour.GetType() == typeof(GlobalMusicController) && ev == Events.AfterStart)
            {
                ModHelper.Console.WriteLine("Swapping _darkBrambleSource in GlobalMusicController");
                behaviour.SetValue("_darkBrambleSource", null);
                _hasBrambleLoaded = true;
            }
            else
            {
                ModHelper.Console.WriteLine("Missing check: " + behaviour.GetType().Name);
            }
        }

        private void Update()
        {
            if (_hasBrambleLoaded)
            {
                var shouldPlay = Locator.GetPlayerSectorDetector().InBrambleDimension() && !Locator.GetPlayerSectorDetector().InVesselDimension() && PlayerState.AtFlightConsole() && !PlayerState.IsHullBreached();
                if (shouldPlay && !_dekuSource.isPlaying)
                {
                    _dekuSource.Play();
                }
                else if (!shouldPlay && _dekuSource.isPlaying)
                {
                    _dekuSource.Stop();
                }
            }
        }

    }

}