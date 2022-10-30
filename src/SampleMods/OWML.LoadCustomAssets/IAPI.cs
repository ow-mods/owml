namespace OWML.LoadCustomAssets
{
	public interface IAPI
	{
		public string Echo(string text);

		public T Radio<T>();
	}
}
