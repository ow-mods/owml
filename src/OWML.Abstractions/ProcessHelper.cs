using System.Diagnostics;
using OWML.Common;

namespace OWML.Abstractions
{
	public class ProcessHelper : IProcessHelper
	{
		public void Start(string path, string[] args = null)
		{
			var processStartInfo = new ProcessStartInfo
			{
				FileName = path,
				UseShellExecute = true
			};

			if (args != null)
			{
				processStartInfo.Arguments = string.Join(" ", args);
			}

			Process.Start(processStartInfo);
		}

		public void KillCurrentProcess() =>
			Process.GetCurrentProcess().Kill();
	}
}
