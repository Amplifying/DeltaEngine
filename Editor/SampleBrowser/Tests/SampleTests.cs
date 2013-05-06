using NUnit.Framework;

namespace DeltaEngine.Editor.SampleBrowser.Tests
{
	public class SampleTests
	{
		[Test]
		public void CreateGameSample()
		{
			var sample = Sample.CreateGame("GameSample", "Game.csproj", "Game.exe");
			Assert.AreEqual("Sample Game", sample.Description);
			Assert.AreEqual(SampleCategory.Game, sample.Category);
			Assert.AreEqual("http://DeltaEngine.net/Content/Icons/GameSample.png", sample.ImageFilePath);
		}

		[Test]
		public void CreateTestSample()
		{
			var sample = Sample.CreateTest("TestSample", "Tests.csproj", "Tests.dll", "ClassName",
				"MethodName");
			Assert.AreEqual("VisualTest", sample.Description);
			Assert.AreEqual(SampleCategory.Test, sample.Category);
			Assert.AreEqual("http://deltaengine.net/Content/Icons/StaticTest.png", sample.ImageFilePath);
		}

		[Test]
		public void ContainsFilterText()
		{
			var gameSample = Sample.CreateGame("GameSample", "Game.csproj", "Game.exe");
			Assert.True(gameSample.ContainsFilterText("Game"));
			Assert.False(gameSample.ContainsFilterText("Test"));
			var testSample = Sample.CreateTest("TestSample", "Tests.csproj", "Tests.dll", "ClassName",
				"MethodName");
			Assert.True(testSample.ContainsFilterText("Test"));
			Assert.False(testSample.ContainsFilterText("Game"));
		}

		[Test]
		public void ToStringTest()
		{
			var sample = Sample.CreateGame(Title, "Game.csproj", "Game.exe");
			Assert.AreEqual(
				"Sample: Title=" + Title + ", Category=" + Category + ", Description=" + Description,
				sample.ToString());
		}

		private const string Title = "Sample's Name";
		private const SampleCategory Category = SampleCategory.Game;
		private const string Description = "Sample Game";
	}
}