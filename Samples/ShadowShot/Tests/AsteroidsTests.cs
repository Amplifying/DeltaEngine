using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace ShadowShot.Tests
{
	public class AsteroidsTests : TestWithMocksOrVisually
	{
		private void Initiliaze()
		{
			Resolve<ScreenSpace>().Window.ViewportPixelSize = new Size(800, 800);
			var image = ContentLoader.Load<Image>("asteroid");
			var drawArea = Rectangle.FromCenter(new Point(0.5f, 0.1f), new Size(0.1f));
			asteroid = new Asteroid(image, drawArea);
		}
		private Asteroid asteroid;

		[Test]
		public void CreateAsteroid(Type resolver)
		{

			Initiliaze();
			Assert.AreEqual(new Point(0.5f, 0.1f), asteroid.DrawArea.Center);
		}

		[Test]
		public void CheckAsteroidFreeFall()
		{

			Initiliaze();
			resolver.AdvanceTimeAndExecuteRunners();
			Assert.LessOrEqual(0.1f, asteroid.DrawArea.Center.Y);
			var changedY = asteroid.DrawArea.Center.Y;
			resolver.AdvanceTimeAndExecuteRunners();
			Assert.LessOrEqual(changedY, asteroid.DrawArea.Center.Y);
		}
	}
}