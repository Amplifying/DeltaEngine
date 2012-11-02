using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Graphics.SharpDX.Tests
{
	public class DeviceTests
	{
		[Test, Category("Slow")]
		public void Dispose()
		{
			App.Start((Device device) => device.Dispose());
		}

		[Test, Category("Slow")]
		public void SizeChanged()
		{
			App.Start((Device device, Window window) => window.TotalSize = new Size(100, 100));
		}
	}
}