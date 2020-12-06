using OWML.Common.Interfaces;
using UnityEngine;

namespace OWML.ModHelper.Input
{
    public class BindingChangeListener : MonoBehaviour
    {
        private ModInputHandler _inputHandler;
        private bool _updateInputsNext, _updateInputs;

        internal void Initialize(ModInputHandler inputHandler, IModEvents events)
        {
            _inputHandler = inputHandler;
            events.Subscribe<TitleScreenManager>(Common.Enums.Events.AfterStart);
            events.Event += OnEvent;
        }

        private void OnEvent(MonoBehaviour behaviour, Common.Enums.Events ev)
        {
            if (behaviour.GetType() == typeof(TitleScreenManager) && ev == Common.Enums.Events.AfterStart)
            {
                _updateInputsNext = true;
            }
        }

        public void Start()
        {
            DontDestroyOnLoad(gameObject);
            GlobalMessenger.AddListener("KeyBindingsChanged", PrepareForUpdate);
        }

        private void PrepareForUpdate()
        {
            _updateInputsNext = true;
        }

        public void Update()
        {
            if (_updateInputs)
            {
                _updateInputs = false;
                UpdateInputs();
            }
            if (_updateInputsNext)
            {
                _updateInputs = true;
                _updateInputsNext = false;
            }
        }

        public void UpdateInputs()
        {
            _inputHandler.UpdateGamesBindings();
        }
    }
}
