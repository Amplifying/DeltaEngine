using DeltaEngine.Content;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace Asteroids.Tests
{
	internal class HudTests : TestWithAllFrameworks
	{
		[Test]
		public void SetScore()
		{
			Start(typeof(MockResolver), (ContentLoader contentLoader, ScreenSpace screenSpace) =>
			{
				var hud = new HudInterface(contentLoader, screenSpace);
				hud.SetScoreText(5);
				Assert.AreEqual("5", hud.ScoreDisplay.Text);
			});
		}
	}
}