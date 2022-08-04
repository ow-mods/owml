using System;

namespace OWML.Common
{
	public interface IModLogger
	{
		[Obsolete("Use ModHelper.Console.WriteLine with messageType = Debug instead.")]
		void Log(string s);

		[Obsolete("Use ModHelper.Console.WriteLine with messageType = Debug instead.")]
		void Log(params object[] s);
	}
}
