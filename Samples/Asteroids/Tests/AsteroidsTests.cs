using System;
using DeltaEngine.Platforms.All;

namespace Asteroids.Tests
{
	public class AsteroidsTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void StartGame(Type resolver)
		{
			Start(resolver, (AsteroidsGame game) => { });
		}
	}
}