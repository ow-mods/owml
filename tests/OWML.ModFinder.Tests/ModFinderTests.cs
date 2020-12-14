using System.Linq;
using OWML.Tests.Setup;
using Xunit;
using Xunit.Abstractions;

namespace OWML.ModFinder.Tests
{
	public class ModFinderTests : OWMLTests
	{
		public ModFinderTests(ITestOutputHelper outputHelper)
			: base(outputHelper)
		{
		}

		[Fact]
		public void ModFinder_GetMods()
		{
			var modFinder = new ModLoader.ModFinder(Config, Console.Object);

			var mods = modFinder.GetMods();

			Assert.True(mods.Count >= 2);
			Assert.Contains("Alek.EnableDebugMode", mods.Select(m => m.Manifest.UniqueName));
			Assert.Contains("Alek.LoadCustomAssets", mods.Select(m => m.Manifest.UniqueName));
		}
	}
}
