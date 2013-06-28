using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace TinyPlatformer.Tests
{
	class ActorTests
	{
		[SetUp]
		public void LoadEntity()
		{
			BlockType[,] blocks = new BlockType[64, 48];
			actor = new Actor(blocks);
		}

		private Actor actor;

		[Test]
		public void ChangeEntityPosition()
		{
			actor.position = new Point(64, 192);
			Assert.AreEqual(new Point(64, 192), actor.position);
		}
	}
}