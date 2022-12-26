using System;
using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper.Events
{
	[Obsolete("Use Harmony patches instead.")]
	public class ModPlayerEvents : IModPlayerEvents
	{
		[Obsolete("Use Harmony patches instead.")]
		public event Action<PlayerBody> OnPlayerAwake;

		public void Init(IModEvents events)
		{
			events.Subscribe<PlayerBody>(Common.Events.AfterAwake);
			events.Event += OnEvent;
		}

		private void OnEvent(MonoBehaviour behaviour, Common.Events ev)
		{
			if (behaviour is PlayerBody playerBody && ev == Common.Events.AfterAwake)
			{
				OnPlayerAwake?.Invoke(playerBody);
			}
		}
	}
}
