using DeltaEngine.Content;
using DeltaEngine.Input;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace FindTheWord.Tests
{
	internal class NextLevelScreenTests : TestWithAllFrameworks
	{
		[Test]
		public void WaitForLevelAdvance()
		{
			Start(typeof(MockResolver), (InputCommands input, ContentLoader content) =>
			{
				var nextLevelScreen = new NextLevelScreen(input, content);
				nextLevelScreen.ShowAndWaitForInput();
				bool levelStartRaised = false;
				nextLevelScreen.StartNextLevel += () => levelStartRaised = true;
				nextLevelScreen.HideAndStartNextLevel();
				Assert.IsTrue(levelStartRaised);
			});
		}
	}
}