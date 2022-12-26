using System;

namespace OWML.Common
{
	[Obsolete("Use Harmony patches instead.")]
	public interface IModPlayerEvents
	{
		[Obsolete("Use Harmony patches instead.")]
		event Action<PlayerBody> OnPlayerAwake;

		void Init(IModEvents events);
	}
}
