using System.Diagnostics;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Graphics.Tests
{
	internal class ScreenshotTests : TestWithMocksOrVisually
	{
		[Test]
		public void MakeScreenshotOfYellowBackground()
		{
			Window.BackgroundColor = Color.Yellow;
			RunCode = () =>
			{
				Resolve<Device>().Present();
				Resolve<ScreenshotCapturer>().MakeScreenshot("Test.png");
				if (!StackTraceExtensions.StartedFromNCrunch)
					Process.Start("Test.png"); //ncrunch: no coverage
			};
			Window.CloseAfterFrame();
		}
	}
}