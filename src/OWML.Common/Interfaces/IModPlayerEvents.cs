using System;

namespace OWML.Common
{
	[Obsolete("Use HarmonyHelper instead.")]
	public interface IModPlayerEvents
	{
		[Obsolete("Use HarmonyHelper instead.")]
		event Action<PlayerBody> OnPlayerAwake;

		void Init(IModEvents events);
	}
}
