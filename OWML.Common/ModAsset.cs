using System;
using UnityEngine;

namespace OWML.Common
{
    public class ModAsset<T> : MonoBehaviour
    {
        public event Action<T> OnLoaded;

        public T Asset { get; private set; }

        public void SetAsset(T asset)
        {
            Asset = asset;
            OnLoaded?.Invoke(asset);
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

    }
}
