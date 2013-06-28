using System;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace FindTheWord.Tests
{
	public class StartupScreenTests : TestWithMocksOrVisually
	{
		[Test]
		public void RaisingGameStarted()
		{
			int isGameStartedCount = 0;
			var screen = Resolve<StartupScreen>();
			screen.GameStarted += () => isGameStartedCount++;
			screen.StartGame();
			Assert.AreEqual(1, isGameStartedCount);
			Window.CloseAfterFrame();
		}

		[Test]
		public void ShowScreen()
		{
			Resolve<StartupScreen>();
		}
	}
}