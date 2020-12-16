using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper.Input
{
	public class BindingChangeListener : MonoBehaviour, IBindingChangeListener
	{
		private IModInputHandler _inputHandler;
		private bool _updateInputsNext, _updateInputs;

		public void Initialize(IModInputHandler inputHandler, IModEvents events)
		{
			_inputHandler = inputHandler;
			events.Subscribe<TitleScreenManager>(Events.AfterStart);
			events.Event += OnEvent;
		}

		private void OnEvent(MonoBehaviour behaviour, Events ev)
		{
			if (behaviour.GetType() == typeof(TitleScreenManager) && ev == Events.AfterStart)
			{
				_updateInputsNext = true;
			}
		}

		public void Start()
		{
			DontDestroyOnLoad(gameObject);
			GlobalMessenger.AddListener("KeyBindingsChanged", PrepareForUpdate);
		}

		private void PrepareForUpdate() => 
			_updateInputsNext = true;

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

		public void UpdateInputs() => 
			_inputHandler.UpdateGamesBindings();
	}
}
