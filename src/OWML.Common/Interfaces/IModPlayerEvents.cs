using System;

namespace OWML.Common
{
	public interface IModPlayerEvents
	{
		event Action<PlayerBody> OnPlayerAwake;

		void Init(IModEvents events);
	}
}
