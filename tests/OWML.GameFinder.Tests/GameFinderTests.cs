using System.IO;
using OWML.Common;
using OWML.Tests.Setup;
using Xunit;
using Xunit.Abstractions;

namespace OWML.GameFinder.Tests
{
    public class GameFinderTests : OWMLTests // todo test steam finder, epic finder etc
    {
        public GameFinderTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        [Fact]
        public void PathFinder_FindGamePath()
        {
            var pathFinder = new PathFinder(new OwmlConfig(), Console.Object);

            var gamePath = pathFinder.FindGamePath();

            Assert.Equal(new DirectoryInfo(Config.GamePath).FullName, new DirectoryInfo(gamePath).FullName);
        }
    }
}
