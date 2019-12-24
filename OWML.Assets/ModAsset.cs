using System;
using UnityEngine;

namespace OWML.Assets
{
    public class ModAsset : MonoBehaviour
    {
        public event Action OnLoaded; 

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

    }
}
