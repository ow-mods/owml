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
		public static readonly Sector.Name Illuminati = (Sector.Name)666;
		public static readonly SignalFrequency Magic;
	}

	public class EnumExample : ModBehaviour
	{
		// Associate an unused enum value with a name of your choosing.
		public static readonly DeathType Example = EnumUtils.Create<DeathType>("Example");
		// Associate a specific enum value with a name of your choosing.
		public static readonly DeathType Example2 = EnumUtils.Create<DeathType>("Example2", -1);
		public static readonly DeathType Example3 = EnumUtils.Create<DeathType>("Example3", 9000);

		public void Start()
		{
			ModHelper.Console.WriteLine($"Checking name of enums", MessageType.Info);

			var createdEnum = EnumUtils.Create<Sector.Name>("Test");
			var createdEnumWithValue = EnumUtils.Create<Sector.Name>(-1, "HahaFunny");


			if (Example.ToString() == nameof(Example))
				ModHelper.Console.WriteLine("Example enum has correct name", MessageType.Success);
			else
				ModHelper.Console.WriteLine("Example enum does not have correct name", MessageType.Error);


			if (Example2.ToString() == nameof(Example2))
				ModHelper.Console.WriteLine("Example2 enum has correct name", MessageType.Success);
			else
				ModHelper.Console.WriteLine("Example2 enum does not have correct name", MessageType.Error);


			if (Example3.ToString() == nameof(Example3))
				ModHelper.Console.WriteLine("Example3 enum has correct name", MessageType.Success);
			else
				ModHelper.Console.WriteLine("Example3 enum does not have correct name", MessageType.Error);


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

			DeathType impact = EnumUtils.Parse<DeathType>("Impact");

			if (EnumUtils.TryParse("Example", out DeathType example))
				ModHelper.Console.WriteLine("Example enum exists", MessageType.Success);
			else
				ModHelper.Console.WriteLine("Example enum does not exist", MessageType.Error);

			int numOfDeathTypes = EnumUtils.Count<DeathType>();
			ModHelper.Console.WriteLine($"Total death types: {numOfDeathTypes}", MessageType.Success);
			DeathType minDeathType = EnumUtils.GetMinValue<DeathType>();
			ModHelper.Console.WriteLine($"Min death type: {minDeathType}", MessageType.Success);
			DeathType maxDeathType = EnumUtils.GetMaxValue<DeathType>();
			ModHelper.Console.WriteLine($"Max death type: {maxDeathType}", MessageType.Success);
			DeathType randomDeathType = EnumUtils.GetRandom<DeathType>();
			ModHelper.Console.WriteLine($"Random death type: {randomDeathType}", MessageType.Success);
			DeathType unusedDeathTypeValue = EnumUtils.GetFirstFreeValue<DeathType>();
			ModHelper.Console.WriteLine($"Unused death type: {unusedDeathTypeValue}", MessageType.Success);
			string[] allDeathNames = EnumUtils.GetNames<DeathType>();
			ModHelper.Console.WriteLine($"All Death Names [{string.Join(", ", allDeathNames)}]", MessageType.Success);
			DeathType[] allDeathValues = EnumUtils.GetValues<DeathType>();
			ModHelper.Console.WriteLine($"All Death Values [{string.Join(", ", allDeathValues.Select(value => (int)value))}]", MessageType.Success);

			bool doesExampleDeathExist = EnumUtils.IsDefined<DeathType>("Example");
			if (doesExampleDeathExist)
				ModHelper.Console.WriteLine("Example enum exists", MessageType.Success);
			else
				ModHelper.Console.WriteLine("Example enum does not exist", MessageType.Error);

			Type valueType = EnumUtils.GetUnderlyingType<DeathType>();
			ModHelper.Console.WriteLine($"Underlying type: {valueType}", MessageType.Success);
			bool hasFlags = EnumUtils.IsPowerOfTwoEnum<DeathType>();
			ModHelper.Console.WriteLine($"Has Flags: {hasFlags}", MessageType.Success);
		}
	}
}