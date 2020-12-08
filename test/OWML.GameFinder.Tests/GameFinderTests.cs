using Moq;
using OWML.Common.Interfaces;
using OWML.Tests.Setup;
using Xunit;
using Xunit.Abstractions;

namespace OWML.GameFinder.Tests
{
    public class GameFinderTests : OWMLTests
    {
        public GameFinderTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        [Fact]
        public void PathFinder_FindGamePath()
        {
            var config = new Mock<IOwmlConfig>();

            var gameFinder = new PathFinder(config.Object, Console.Object);
            var gamePath = gameFinder.FindGamePath();

            const string expectedPath = "C:\\Program Files\\Epic Games\\OuterWilds";

            Assert.Equal(expectedPath, gamePath);
        }
    }
}
