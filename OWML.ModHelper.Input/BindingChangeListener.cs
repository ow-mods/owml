using OWML.ModHelper.Events;
using UnityEngine;

namespace OWML.ModHelper.Input
{
    public class BindingChangeListener : MonoBehaviour
    {
        private ModInputHandler _inputHandler;
        private bool _updateInputsNext, _updateInputs;

        internal void Initialize(ModInputHandler inputHandler)
        {
            _inputHandler = inputHandler;
            var events = ModEvents.Instance;
            events.Subscribe<TitleScreenManager>(Common.Events.AfterStart);
            events.Event += OnEvent;
        }

        private void OnEvent(MonoBehaviour behaviour, Common.Events ev)
        {
            if (behaviour.GetType() == typeof(TitleScreenManager) && ev == Common.Events.AfterStart)
            {
                _updateInputsNext = true;
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            GlobalMessenger.AddListener("KeyBindingsChanged", PrepareForUpdate);
        }

        private void PrepareForUpdate()
        {
            _updateInputsNext = true;
        }

        private void Update()
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
