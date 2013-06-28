using System;
using DeltaEngine.Platforms;
using DeltaEngine.Input;
using NUnit.Framework;

namespace TinyPlatformer.Tests
{
	class GameLogicTests : TestWithMocksOrVisually
	{
		[Test]
		public void Create(Type resolver)
		{
				var entityUpdater = new GameLogic(Resolve<Map>(), Resolve<InputCommands>());
				Assert.AreEqual(entityUpdater.player.Type, "player");
		}
	}
}