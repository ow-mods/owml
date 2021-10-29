using UnityEngine;

namespace OWML.ModHelper
{
	public class OwmlBehaviour : MonoBehaviour
	{
		public void Start() => DontDestroyOnLoad(gameObject);
	}
}
