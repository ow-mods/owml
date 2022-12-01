using System;
using System.Collections;
using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper.Events
{
	public class ModUnityEvents : MonoBehaviour, IModUnityEvents
	{
		[Obsolete("Use HarmonyHelper instead.")]
		public event Action OnUpdate;

		[Obsolete("Use HarmonyHelper instead.")]
		public event Action OnFixedUpdate;

		[Obsolete("Use HarmonyHelper instead.")]
		public event Action OnLateUpdate;

		public void FireOnNextUpdate(Action action) => 
			FireInNUpdates(action, 1);

		public void FireInNUpdates(Action action, int n) => 
			StartCoroutine(WaitForFrames(action, n));

		public void RunWhen(Func<bool> predicate, Action action) => 
			StartCoroutine(WaitUntil(predicate, action));

		private IEnumerator WaitForFrames(Action action, int n)
		{
			for (var i = 0; i < n; i++)
			{
				yield return new WaitForEndOfFrame();
			}
			action.Invoke();
		}

		private IEnumerator WaitUntil(Func<bool> predicate, Action action)
		{
			yield return new WaitUntil(predicate);
			action();
		}

		public void Start() => 
			DontDestroyOnLoad(gameObject);

		public void Update() => 
			OnUpdate?.Invoke();

		public void FixedUpdate() => 
			OnFixedUpdate?.Invoke();

		public void LateUpdate() => 
			OnLateUpdate?.Invoke();
	}
}
