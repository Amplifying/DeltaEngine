using System;
using DeltaEngine.Platforms.All;
using NUnit.Framework;

namespace FindTheWord.Tests
{
	public class StartupScreenTests : TestWithAllFrameworks
	{
		[IntegrationTest]
		public void RaisingGameStarted(Type resolver)
		{
			int isGameStartedCount = 0;
			Start(resolver, (StartupScreen screen) =>
			{
				screen.GameStarted += () => isGameStartedCount++;
				screen.StartGame();
				Assert.AreEqual(1, isGameStartedCount);
			});
		}

		[VisualTest]
		public void ShowScreen(Type resolver)
		{
			Start(resolver, (StartupScreen screen) => { });
		}
	}
}