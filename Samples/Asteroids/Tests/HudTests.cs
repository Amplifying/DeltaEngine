using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace Asteroids.Tests
{
	internal class HudTests : TestWithMocksOrVisually
	{
		[Test]
		public void SetScore()
		{
			var hud = new HudInterface(Resolve<ScreenSpace>());
			hud.SetScoreText(5);
			Assert.AreEqual("5", hud.ScoreDisplay.Text);
		}
	}
}