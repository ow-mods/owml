using System;

namespace OWML.Common
{
	public interface IModUnityEvents
	{
		[Obsolete("Use Harmony patches instead.")]
		event Action OnUpdate;

		[Obsolete("Use Harmony patches instead.")]
		event Action OnFixedUpdate;

		[Obsolete("Use Harmony patches instead.")]
		event Action OnLateUpdate;

		void FireOnNextUpdate(Action action);

		void FireInNUpdates(Action action, int n);

		void RunWhen(Func<bool> predicate, Action action);
	}
}
