#if TODO
using DeltaEngine.Datatypes;
using DeltaEngine.Scenes.UserInterfaces;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.UserInterfaces
{
	public class TileTests
	{
		[Test]
		public void CreateTileWithDefaultType()
		{
			var tile = new Tile(1, 2);
			Assert.AreEqual(1, tile.Column);
			Assert.AreEqual(2, tile.Row);
			Assert.AreEqual(Tile.DefaultTileType, tile.Type);
			Assert.AreEqual(Rectangle.Zero, tile.Sprite.DrawArea);
			Assert.AreEqual(null, tile.Sprite.Image);
		}

		[Test]
		public void CreateTilePassingInType()
		{
			var tile = new Tile(1, 1, "NewType");
			Assert.AreEqual("NewType", tile.Type);
		}

		[Test]
		public void SetTypeProperty()
		{
			var tile = new Tile(1, 1) { Type = "NewType" };
			Assert.AreEqual("NewType", tile.Type);
		}
	}
}
#endif