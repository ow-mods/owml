using OWML.Common;
using OWML.ModHelper;
using OWML.Utils;

namespace OWML.EnumExample
{
	[EnumHolder]
	public static class CustomEnums
	{
		public static Sector.Name Illuminati = (Sector.Name)666;
		public static SignalFrequency Magic;
	}

	public class EnumExample : ModBehaviour
	{
		public void Start()
		{
			ModHelper.Console.WriteLine($"Checking name of enums", MessageType.Info);

			if (CustomEnums.Illuminati.ToString() == nameof(CustomEnums.Illuminati))
			{
				ModHelper.Console.WriteLine("Illuminati enum has correct name", MessageType.Success);
				if ((int)CustomEnums.Illuminati == 666)
					ModHelper.Console.WriteLine("Illuminati enum has correct value", MessageType.Success);
				else
					ModHelper.Console.WriteLine("Illuminati enum does not have correct value", MessageType.Error);
			}
			else
				ModHelper.Console.WriteLine("Illuminati enum does not have correct name", MessageType.Error);

			if (CustomEnums.Magic.ToString() == nameof(CustomEnums.Magic))
				ModHelper.Console.WriteLine("Magic enum has correct name", MessageType.Success);
			else
				ModHelper.Console.WriteLine("Magic enum does not have correct name", MessageType.Error);

			var createdEnum = EnumUtils.Create<Sector.Name>("Test");
			if (createdEnum.ToString() == "Test")
			{
				ModHelper.Console.WriteLine("Test enum has correct name", MessageType.Success);

				EnumUtils.Remove<Sector.Name>(createdEnum);
				if (createdEnum.ToString() == "Test")
					ModHelper.Console.WriteLine("Test enum was not removed", MessageType.Error);
				else
					ModHelper.Console.WriteLine("Test enum was removed", MessageType.Success);
			}
			else
				ModHelper.Console.WriteLine("Test enum does not have correct name", MessageType.Error);
		}
	}
}