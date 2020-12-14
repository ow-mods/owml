namespace OWML.Common
{
	public interface IProcessHelper
	{
		void Start(string path, string[] args);

		void KillCurrentProcess();
	}
}