using Moq;
using OWML.Common;
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
        public void ModLoader_LoadMods() // fails because i removed copying mod to release folder
        {
            var container = ModLoader.CreateContainer(AppHelper.Object, GOHelper.Object);
            container.Add(Console.Object);
            container.Add(Logger.Object);
            container.Add(Config);

            var owo = container.Resolve<Owo>();
            owo.LoadMods();

            Mod.Verify(s => s.Init(It.IsAny<IModHelper>()), Times.Once());
        }

        [Fact]
        public void ModLoader_OldVersion_LoadMods() // fails because i removed copying mod to release folder
        {
            Config.GamePath = "C:/Program Files (x86)/Outer Wilds";

            var container = ModLoader.CreateContainer(AppHelper.Object, GOHelper.Object);
            container.Add(Console.Object);
            container.Add(Logger.Object);
            container.Add(Config);

            var owo = container.Resolve<Owo>();
            owo.LoadMods();

            Mod.Verify(s => s.Init(It.IsAny<IModHelper>()), Times.Once());
        }
    }
}
