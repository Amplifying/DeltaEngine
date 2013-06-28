using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Graphics.Tests
{
	public class DeviceTests : TestWithMocksOrVisually
	{
		[Test, ApproveFirstFrameScreenshot]
		public void DrawRedBackground()
		{
			Window.BackgroundColor = Color.Red;
		}

		[Test]
		public void SizeChanged()
		{
			Window.ViewportPixelSize = new Size(200, 100);
			Assert.AreEqual(new Size(200, 100), Window.ViewportPixelSize);
		}

		[Test]
		public void CloseAfterOneFrame()
		{
			Window.CloseAfterFrame();
		}

		[Test, ApproveFirstFrameScreenshot]
		public void SetFullscreenModeAndShowRedBackground()
		{
			Settings.StartInFullscreen = true;
			Window.BackgroundColor = Color.Red;
			Settings.StartInFullscreen = false;
		}
	}
}