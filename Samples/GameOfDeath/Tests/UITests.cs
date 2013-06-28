using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace GameOfDeath.Tests
{
	internal class UITests : TestWithMocksOrVisually
	{
		[Test]
		public void ShowBackgroundWithUI()
		{
			Resolve<UI>();
		}

		[Test]
		public void Resize()
		{
			Resolve<UI>();
			Window.ViewportPixelSize = new Size(800, 600);
		}
	}
}