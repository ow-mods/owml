using System;

namespace OWML.Common
{
	public interface IModUnityEvents
	{
		[Obsolete("Use HarmonyHelper instead.")]
		event Action OnUpdate;

		[Obsolete("Use HarmonyHelper instead.")]
		event Action OnFixedUpdate;

		[Obsolete("Use HarmonyHelper instead.")]
		event Action OnLateUpdate;

		void FireOnNextUpdate(Action action);

		void FireInNUpdates(Action action, int n);

		void RunWhen(Func<bool> predicate, Action action);
	}
}
