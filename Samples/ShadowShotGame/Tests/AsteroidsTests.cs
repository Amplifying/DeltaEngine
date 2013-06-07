using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;
using ShadowShotGame;

namespace ShadowShotGameTests
{
	public class AsteroidsTests : TestWithAllFrameworks
	{
		private void Initiliaze(ContentLoader contentLoader, ScreenSpace screenSpace)
		{
			content = contentLoader;
			screen = screenSpace;
			screen.Window.TotalPixelSize = new Size(800, 800);
			var image = content.Load<Image>("asteroid");
			var drawArea = Rectangle.FromCenter(new Point(0.5f, 0.1f), new Size(0.1f));
			asteroid = new Asteroid(image, drawArea);
		}

		private ContentLoader content;
		private ScreenSpace screen;
		private Asteroid asteroid;

		[VisualTest]
		public void CreateAsteroid(Type resolver)
		{
			Start(resolver, (ContentLoader contentLoader, ScreenSpace screenSpace) =>
			{
				Initiliaze(contentLoader, screenSpace);
				Assert.AreEqual(new Point(0.5f, 0.1f), asteroid.DrawArea.Center);
			});
		}

		[Test]
		public void CheckAsteroidFreeFall()
		{
			Start(typeof(MockResolver), (ContentLoader contentLoader, ScreenSpace screenSpace) =>
			{
				Initiliaze(contentLoader, screenSpace);
				mockResolver.AdvanceTimeAndExecuteRunners();
				Assert.LessOrEqual(0.1f, asteroid.DrawArea.Center.Y);
				var changedY = asteroid.DrawArea.Center.Y;
				mockResolver.AdvanceTimeAndExecuteRunners();
				Assert.LessOrEqual(changedY, asteroid.DrawArea.Center.Y);
			});
		}
	}
}