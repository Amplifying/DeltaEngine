using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.All;
using DeltaEngine.Scenes.UserInterfaces;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.UserInterfaces
{
	public class TileTests : TestWithAllFrameworks
	{
		[IntegrationTest]
		public void CreateTileWithDefaultType(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var image = content.Load<Image>("DeltaEngineLogo");
				var tile = new Tile(image, 1, 2);
				Assert.AreEqual(1, tile.Column);
				Assert.AreEqual(2, tile.Row);
				Assert.AreEqual(new Point(1, 2), tile.TileCoord);
				Assert.AreEqual(Rectangle.Zero, tile.DrawArea);
				Assert.AreEqual(image, tile.Image);
			});
		}
	}
}