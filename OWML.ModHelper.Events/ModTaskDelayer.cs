using System;
using UnityEngine;

namespace OWML.ModHelper.Events
{
    public class ModTaskDelayer : MonoBehaviour
    {
        public event Action OnNextUpdate;

        public bool FireEventOnNextUpdate { get; set; }

        private void Update()
        {
            if (FireEventOnNextUpdate)
            {
                FireEventOnNextUpdate = false;
                OnNextUpdate?.Invoke();
            }
        }
    }
}
