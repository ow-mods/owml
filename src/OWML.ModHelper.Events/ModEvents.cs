using System;
using System.Collections.Generic;
using System.Linq;
using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper.Events
{
	public class ModEvents : IModEvents
	{
		public IModPlayerEvents Player { get; }

		public IModSceneEvents Scenes { get; }

		public IModUnityEvents Unity { get; }

		public event Action<MonoBehaviour, Common.Events> Event;

		private static readonly List<KeyValuePair<Type, Common.Events>> PatchedEvents = new();
		private readonly List<KeyValuePair<Type, Common.Events>> _subscribedEvents = new();

		private readonly IHarmonyHelper _harmonyHelper;
		private readonly IModConsole _console;

		public ModEvents(
			IModConsole console,
			IHarmonyHelper harmonyHelper,
			IModPlayerEvents playerEvents,
			IModSceneEvents sceneEvents,
			IModUnityEvents unityEvents)
		{
			_console = console;
			_harmonyHelper = harmonyHelper;
			Player = playerEvents;
			Scenes = sceneEvents;
			Unity = unityEvents;

			Player.Init(this);
			Patches.OnEvent += OnPatchEvent;
		}

		private void OnPatchEvent(MonoBehaviour behaviour, Common.Events ev)
		{
			var type = behaviour.GetType();
			if (IsSubscribedTo(type, ev))
			{
				_console.WriteLine($"Got subscribed event: {ev} of {type.Name}", MessageType.Debug);
				Event?.Invoke(behaviour, ev);
			}
			else
			{
				_console.WriteLine($"Not subscribed to: {ev} of {type.Name}", MessageType.Debug);
			}
		}

		public void Subscribe<T>(Common.Events ev) where T : MonoBehaviour
		{
			SubscribeToEvent<T>(ev);
			PatchEvent<T>(ev);
		}

		private void SubscribeToEvent<T>(Common.Events ev)
		{
			var type = typeof(T);
			if (IsSubscribedTo(type, ev))
			{
				_console.WriteLine($"Already subscribed to {ev} of {type.Name}", MessageType.Debug);
				return;
			}
			AddToEventList(_subscribedEvents, type, ev);
		}

		private void PatchEvent<T>(Common.Events ev)
		{
			var type = typeof(T);
			if (InEventList(PatchedEvents, type, ev))
			{
				_console.WriteLine($"Event is already patched: {ev} of {type.Name}", MessageType.Debug);
				return;
			}
			AddToEventList(PatchedEvents, type, ev);

			switch (ev)
			{
				case Common.Events.BeforeAwake:
					_harmonyHelper.AddPrefix<T>("Awake", typeof(Patches), nameof(Patches.BeforeAwake));
					break;
				case Common.Events.AfterAwake:
					_harmonyHelper.AddPostfix<T>("Awake", typeof(Patches), nameof(Patches.AfterAwake));
					break;

				case Common.Events.BeforeStart:
					_harmonyHelper.AddPrefix<T>("Start", typeof(Patches), nameof(Patches.BeforeStart));
					break;
				case Common.Events.AfterStart:
					_harmonyHelper.AddPostfix<T>("Start", typeof(Patches), nameof(Patches.AfterStart));
					break;

				case Common.Events.BeforeEnable:
					_harmonyHelper.AddPrefix<T>("OnEnable", typeof(Patches), nameof(Patches.BeforeEnable));
					break;
				case Common.Events.AfterEnable:
					_harmonyHelper.AddPostfix<T>("OnEnable", typeof(Patches), nameof(Patches.AfterEnable));
					break;

				case Common.Events.BeforeDisable:
					_harmonyHelper.AddPrefix<T>("OnDisable", typeof(Patches), nameof(Patches.BeforeDisable));
					break;
				case Common.Events.AfterDisable:
					_harmonyHelper.AddPostfix<T>("OnDisable", typeof(Patches), nameof(Patches.AfterDisable));
					break;

				case Common.Events.BeforeDestroy:
					_harmonyHelper.AddPrefix<T>("OnDestroy", typeof(Patches), nameof(Patches.BeforeDestroy));
					break;
				case Common.Events.AfterDestroy:
					_harmonyHelper.AddPostfix<T>("OnDestroy", typeof(Patches), nameof(Patches.AfterDestroy));
					break;

				default:
					_console.WriteLine($"Error - Unrecognized event: {ev}", MessageType.Error);
					throw new ArgumentOutOfRangeException(nameof(ev), ev, null);
			}
		}

		private bool IsSubscribedTo(Type type, Common.Events ev) => 
			_subscribedEvents.Any(pair => (type == pair.Key || type.IsSubclassOf(pair.Key)) && pair.Value == ev);

		private bool InEventList(List<KeyValuePair<Type, Common.Events>> events, Type type, Common.Events ev) => 
			events.Any(pair => type == pair.Key && pair.Value == ev);

		private void AddToEventList(List<KeyValuePair<Type, Common.Events>> events, Type type, Common.Events ev) => 
			events.Add(new KeyValuePair<Type, Common.Events>(type, ev));
	}
}
