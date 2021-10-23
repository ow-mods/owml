using System.Diagnostics;
using OWML.Common;

namespace OWML.Abstractions
{
	public class ProcessHelper : IProcessHelper
	{
		public void Start(string path, string[] args = null)
		{
			if (args is null)
			{
				Process.Start(path);
				return;
			}

			Process.Start(path, string.Join(" ", args));
		}

		public void KillCurrentProcess() =>
			Process.GetCurrentProcess().Kill();
	}
}
