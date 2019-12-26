using System;
using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper.Assets
{
    public class ModAsset<T> : MonoBehaviour, IModAsset<T>
    {
        public Action<T> OnLoaded { get; set; }

        public T Asset { get; private set; }

        public void SetAsset(T asset)
        {
            Asset = asset;
            if (OnLoaded == null)
            {
                ModBehaviour.ModHelper.Console.WriteLine("Invoking OnLoaded with no subscribers :(");
            }
            OnLoaded?.Invoke(asset);
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public T1 AddComponent<T1>() where T1 : Component
        {
            return gameObject.AddComponent<T1>();
        }

    }
}
