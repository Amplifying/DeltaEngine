using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;

namespace DeltaEngine.Graphics.Tests
{
	public class DeviceTests : TestStarter
	{
		[IntegrationTest]
		public void Dispose(Type resolver)
		{
			Start(resolver, (Device device) => device.Dispose());
		}

		[VisualTest]
		public void DrawRedBackground(Type resolver)
		{
			Start(resolver, (Device device, Window window) => window.BackgroundColor = Color.Red);
		}

		[IntegrationTest]
		public void Present(Type resolver)
		{
			Start(resolver, (Device device) => device.Present());
		}

		[VisualTest]
		public void SizeChanged(Type resolver)
		{
			Start(resolver, (Device device, Window window) => window.TotalPixelSize = new Size(100, 100));
		}
	}
}