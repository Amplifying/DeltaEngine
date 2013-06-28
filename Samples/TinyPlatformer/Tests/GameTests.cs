using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace TinyPlatformer.Tests
{
	class GameTests : TestWithMocksOrVisually
	{
		[Test]
		public void Create()
		{
			Resolve<Game>();
		}
	}
}