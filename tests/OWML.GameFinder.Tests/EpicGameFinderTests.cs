using System.IO;
using OWML.Common;
using OWML.Tests.Setup;
using Xunit;
using Xunit.Abstractions;

namespace OWML.GameFinder.Tests
{
	public class EpicGameFinderTests : OWMLTests
	{
		public EpicGameFinderTests(ITestOutputHelper outputHelper)
			: base(outputHelper)
		{
		}

		[Fact]
		public void EpicGameFinder_FindGamePath()
		{
			var pathFinder = new EpicGameFinder(new OwmlConfig(), Console.Object);

			var gamePath = pathFinder.FindGamePath();

			Assert.Equal(new DirectoryInfo(Config.GamePath).FullName, new DirectoryInfo(gamePath).FullName);
		}
	}
}
