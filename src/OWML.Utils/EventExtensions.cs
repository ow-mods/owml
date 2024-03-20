using System;
using System.Reflection;
using OWML.Common;
using OWML.Logging;

namespace OWML.Utils
{
	public static class EventExtensions
	{
		/// <summary>
		/// Invokes each delegate, printing an error if an invocation fails. If an invocation fails, the other delegates will still be invoked.
		/// </summary>
		/// <param name="multicast">The MulticastDelegate to invoke.</param>
		/// <param name="args">The arguments to pass to each invocation.</param>
		public static void SafeInvoke(this MulticastDelegate multicast, params object[] args)
		{
			foreach (var del in multicast.GetInvocationList())
			{
				try
				{
					del.DynamicInvoke(args);
				}
				catch (TargetInvocationException ex)
				{
					ModConsole.OwmlConsole.WriteLine($"Error invoking delegate! {ex.InnerException}", MessageType.Error);
				}
			}
		}

		/// <summary>
		/// Raises an event in an instance by it's name.
		/// </summary>
		/// <typeparam name="T">The type of the instance.</typeparam>
		/// <param name="instance">The instance to raise the event in.</param>
		/// <param name="eventName">The name of the event.</param>
		/// <param name="args">The arguments to be passed to the event.</param>
		public static void RaiseEvent<T>(this T instance, string eventName, params object[] args)
		{
			const BindingFlags flags = BindingFlags.Instance
			                           | BindingFlags.Static
			                           | BindingFlags.Public
			                           | BindingFlags.NonPublic
			                           | BindingFlags.DeclaredOnly;
			if (typeof(T)
				    .GetField(eventName, flags)?
				    .GetValue(instance) is not MulticastDelegate multiDelegate)
			{
				return;
			}

			multiDelegate.SafeInvoke(args);
		}
	}
}
