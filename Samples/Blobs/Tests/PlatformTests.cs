using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace Blobs.Tests
{
	public class PlatformTests : TestWithMocksOrVisually
	{
		[Test]
		public void DrawRotatingPlatform()
		{
			new Platform(new Rectangle(0.3f, 0.3f, 0.3f, 0.2f)) { Color = Color.Gold };
		}
	}
}