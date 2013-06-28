using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace TinyPlatformer.Tests
{
	class MapTests
	{
		[SetUp]
		public void LoadMap()
		{			
			map = new Map();
		}

		private Map map;

		[Test]
		public void CheckMapSize()
		{
			Assert.AreEqual(new Size(64, 48), map.Size);
		}

		[Test]
		public void CheckMapContent()
		{
			Assert.AreEqual(BlockType.LevelBorder, map.Blocks[0, 0]);
			Assert.AreEqual(BlockType.None, map.Blocks[1, 1]);
			Assert.AreEqual(BlockType.PlatformBrick, map.Blocks[1, 8]);
		}

		[Test]
		public void CheckEntityLoading()
		{
			Assert.AreEqual("player", map.ActorList.Find(e => e.Type == "player").Type);
			Assert.AreEqual("monster", map.ActorList.Find(e => e.Type == "monster").Type);
			Assert.AreEqual("treasure", map.ActorList.Find(e => e.Type == "treasure").Type);
		}

	}
}
