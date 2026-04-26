using System;
using System.Runtime.InteropServices;

namespace OWML.Common
{
	public static class WineChecker
	{
		[DllImport("ntdll.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr wine_get_version();

		public static bool IsUsingWine()
		{
			try
			{
				return wine_get_version() != IntPtr.Zero;
			}
			catch (EntryPointNotFoundException)
			{
				return false;
			}
		}
	}
}
