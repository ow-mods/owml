using System;
using UnityEngine;

namespace OWML.Common
{
	[Obsolete("Use HarmonyHelper instead.")]
	public interface IModEvents
	{
		[Obsolete("Use HarmonyHelper instead.")]
		IModPlayerEvents Player { get; }

		[Obsolete("Use HarmonyHelper instead.")]
		IModSceneEvents Scenes { get; }

		[Obsolete("Use HarmonyHelper instead.")]
		IModUnityEvents Unity { get; }

		[Obsolete("Use HarmonyHelper instead.")]
		event Action<MonoBehaviour, Events> Event;

		[Obsolete("Use Event instead.")]
		Action<MonoBehaviour, Events> OnEvent { get; set; }

		void Subscribe<T>(Events ev) where T : MonoBehaviour;
	}
}
