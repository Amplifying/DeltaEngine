using DeltaEngine.Input;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace FindTheWord.Tests
{
	internal class NextLevelScreenTests : TestWithMocksOrVisually
	{
		[Test]
		public void WaitForLevelAdvance()
		{
			var nextLevelScreen = new NextLevelScreen(Resolve<InputCommands>());
			nextLevelScreen.ShowAndWaitForInput();
			bool levelStartRaised = false;
			nextLevelScreen.StartNextLevel += () => levelStartRaised = true;
			nextLevelScreen.HideAndStartNextLevel();
			Assert.IsTrue(levelStartRaised);
		}
	}
}