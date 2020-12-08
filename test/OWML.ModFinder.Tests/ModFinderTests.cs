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
            var modFinder = new ModLoader.ModFinder(Config.Object, Console.Object);

            var mods = modFinder.GetMods();

            Assert.Equal(2, mods.Count);
            Assert.Equal("Alek.EnableDebugMode", mods[0].Manifest.UniqueName);
            Assert.Equal("Alek.LoadCustomAssets", mods[1].Manifest.UniqueName);
        }
    }
}
