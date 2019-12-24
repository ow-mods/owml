using System;
using UnityEngine;

namespace OWML.Common
{
    public class ModAsset<T> : MonoBehaviour
    {
        private T _asset;
        public event Action<T> OnLoaded;

        public T Asset
        {
            get => _asset;
            set
            {
                _asset = value; 
                OnLoaded?.Invoke(_asset);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

    }
}
