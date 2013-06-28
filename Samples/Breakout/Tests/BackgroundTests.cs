using System;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering;
using NUnit.Framework;

namespace Breakout.Tests
{
	public class BackgroundTests : TestWithMocksOrVisually
	{
		[Test]
		public void Draw()
		{
			var background = Resolve<Background>();
			Assert.IsTrue(background.Visibility == Visibility.Show);
		}
	}
}