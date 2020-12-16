using UnityEngine;

namespace OWML.Common
{
	public class DontDestroyOnLoad : MonoBehaviour
	{
		public void Start() =>
			DontDestroyOnLoad(gameObject);
	}
}
