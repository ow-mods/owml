using OWML.ModHelper;

namespace OWML.ExampleAPI
{
	public class ExampleAPI : ModBehaviour
	{
		public override object GetApi() => new API();
	}

	public class API
	{
		public string Echo(string text)
			=> text;

		public T Radio<T>()
			=> default;
	}
}