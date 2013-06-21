using System;
using System.Diagnostics;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;

namespace DeltaEngine.Graphics.Tests
{
	internal class ScreenshotTests : TestWithAllFrameworks
	{
		[IntegrationTest]
		public void MakeScreenshotofYelloyBackground(Type resolver)
		{
			Start(resolver, (Device device, Window window) => { window.BackgroundColor = Color.Yellow; },
				(ScreenshotCapturer capturer) =>
				{
					capturer.MakeScreenshot("Test.png");
					if (resolver != typeof(MockResolver))
						Process.Start("Test.png"); //ncrunch: no coverage
				});
		}
	}
}