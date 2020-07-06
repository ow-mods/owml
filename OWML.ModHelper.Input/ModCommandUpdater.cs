using System;
using UnityEngine;

namespace OWML.ModHelper.Input
{
    public class ModCommandUpdater : MonoBehaviour
    {
        private InputCommand _command;

        public void Initialize(InputCommand command)
        {
            _command = command;
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            _command?.UpdateInputCommand();
        }
    }
}