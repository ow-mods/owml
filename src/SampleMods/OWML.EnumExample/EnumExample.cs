using OWML.Common;
using OWML.ModHelper;
using OWML.Utils;
using System;
using System.Linq;

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

			var createdEnum = EnumUtils.Create<Sector.Name>("Test");


			if (CustomEnums.Magic.ToString() == nameof(CustomEnums.Magic))
				ModHelper.Console.WriteLine("Magic enum has correct name", MessageType.Success);
			else
				ModHelper.Console.WriteLine("Magic enum does not have correct name", MessageType.Error);


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


			if (Enum.IsDefined(typeof(Sector.Name), 666))
				ModHelper.Console.WriteLine("Illuminati enum is defined", MessageType.Success);
			else
				ModHelper.Console.WriteLine("Illuminati enum is not defined", MessageType.Error);


			var names = Enum.GetNames(typeof(Sector.Name));
			if (names.Contains("Illuminati") && names.Contains("Test"))
				ModHelper.Console.WriteLine("Sector.Name contains Illuminati and Test", MessageType.Success);
			else
				ModHelper.Console.WriteLine("Sector.Name does not contain Illuminati and Test", MessageType.Error);


			var values = Enum.GetValues(typeof(Sector.Name)).Cast<int>();
			if (values.Contains(666) && values.Contains((int)createdEnum))
				ModHelper.Console.WriteLine("Sector.Name contains values 666 and " + ((int)createdEnum), MessageType.Success);
			else
				ModHelper.Console.WriteLine("Sector.Name does not contain values 666 and " + ((int)createdEnum), MessageType.Error);


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