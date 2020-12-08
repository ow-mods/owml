using Moq;
using OWML.Common.Interfaces;
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
        public void LoadMods_LoadsMods()
        {
            var container = ModLoader.CreateContainer(AppHelper.Object, GOHelper.Object);
            container.Add(Console.Object);
            container.Add(Logger.Object);
            container.Add(Config.Object);

            var owo = container.Resolve<Owo>();
            owo.LoadMods();

            Mod.Verify(s => s.Init(It.IsAny<IModHelper>()), Times.Once());
        }
    }
}
