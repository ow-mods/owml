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

		public void Start()
		{
			DontDestroyOnLoad(gameObject);
		}

		public void Update()
		{
			_command?.UpdateInputCommand();
		}
	}
}