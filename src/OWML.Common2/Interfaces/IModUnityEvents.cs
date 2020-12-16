using System;

namespace OWML.Common
{
	public interface IModUnityEvents
	{
		event Action OnUpdate;

		event Action OnFixedUpdate;

		event Action OnLateUpdate;

		void FireOnNextUpdate(Action action);

		void FireInNUpdates(Action action, int n);

		void RunWhen(Func<bool> predicate, Action action);
	}
}
