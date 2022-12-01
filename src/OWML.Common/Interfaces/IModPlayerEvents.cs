using System;

namespace OWML.Common
{
	public interface IModPlayerEvents
	{
		[Obsolete("Use HarmonyHelper instead.")]
		event Action<PlayerBody> OnPlayerAwake;

		void Init(IModEvents events);
	}
}
