using NUnit.Framework;

namespace DeltaEngine.Editor.SampleBrowser.Tests
{
	public class SampleTests
	{
		[Test]
		public void CreateGameSample()
		{
			var sample = new Sample("GameSample", SampleCategory.Game, "Game.sln", "Game.csproj",
				"Game.exe");
			Assert.AreEqual("Sample Game", sample.Description);
			Assert.AreEqual(SampleCategory.Game, sample.Category);
			Assert.AreEqual("http://DeltaEngine.net/Editor/Icons/GameSample.png", sample.ImageUrl);
		}

		[Test]
		public void CreateTestSample()
		{
			var sample = new Sample("TestSample", SampleCategory.Test, "Test.sln", "Tests.csproj",
				"Tests.dll") { EntryClass = "ClassName", EntryMethod = "MethodName" };
			Assert.AreEqual("Visual Test", sample.Description);
			Assert.AreEqual(SampleCategory.Test, sample.Category);
			Assert.AreEqual("http://DeltaEngine.net/Editor/Icons/VisualTest.png", sample.ImageUrl);
		}

		[Test]
		public void ContainsFilterText()
		{
			var gameSample = new Sample("GameSample", SampleCategory.Game, "Game.sln", "Game.csproj",
				"Game.exe");
			Assert.True(gameSample.ContainsFilterText("Game"));
			Assert.False(gameSample.ContainsFilterText("Test"));
			var testSample = new Sample("TestSample", SampleCategory.Test, "Test.sln", "Tests.csproj",
				"Tests.dll") { EntryClass = "ClassName", EntryMethod = "MethodName" };
			Assert.True(testSample.ContainsFilterText("Test"));
			Assert.False(testSample.ContainsFilterText("Game"));
		}

		[Test]
		public void ContainsTutorialFilterText()
		{
			var gameSample = new Sample("TutorialSample", SampleCategory.Tutorial, "Tutorial.sln",
				"Tutorial.csproj", "Tutorial.exe");
			Assert.True(gameSample.ContainsFilterText("Tutorial"));
		}

		[Test]
		public void ToStringTest()
		{
			var sample = new Sample(Title, SampleCategory.Game, "Game.sln", "Game.csproj", "Game.exe");
			Assert.AreEqual(
				"Sample: Title=" + Title + ", Category=" + Category + ", Description=" + Description,
				sample.ToString());
		}

		private const string Title = "Sample's Name";
		private const SampleCategory Category = SampleCategory.Game;
		private const string Description = "Sample Game";
	}
}