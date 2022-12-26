using System;
using UnityEngine;

namespace OWML.Common
{
	public interface IModEvents
	{
		[Obsolete("Use HarmonyHelper instead.")]
		IModPlayerEvents Player { get; }

		IModSceneEvents Scenes { get; }

		IModUnityEvents Unity { get; }

		[Obsolete("Use HarmonyHelper instead.")]
		event Action<MonoBehaviour, Events> Event;

		[Obsolete("Use Event instead.")]
		Action<MonoBehaviour, Events> OnEvent { get; set; }

		void Subscribe<T>(Events ev) where T : MonoBehaviour;
	}
}
