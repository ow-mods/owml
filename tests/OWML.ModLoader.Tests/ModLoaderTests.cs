using OWML.Tests.Setup;
using Xunit;
using Xunit.Abstractions;

namespace OWML.ModLoader.Tests
{
	public class ModLoaderTests : OWMLTests
	{
		public ModLoaderTests(ITestOutputHelper outputHelper)
			: base(outputHelper)
		{
		}

		[Fact]
		public void ModLoader_LoadMods()
		{
			var container = ModLoader.CreateContainer(AppHelper.Object, GOHelper.Object);
			container.Add(Console.Object);
			container.Add(Config);

			var owo = container.Resolve<Owo>();
			owo.LoadMods();
		}
	}
}
