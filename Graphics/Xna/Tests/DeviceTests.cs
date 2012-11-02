using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Graphics.Xna.Tests
{
	public class DeviceTests
	{
		[Test, Category("Slow")]
		public void Dispose()
		{
			App.Start((Device device, Window window) =>
			{
				device.Dispose();
				window.Dispose();
			});
		}

		[Test, Category("Slow")]
		public void ShowWindowWithViewport()
		{
			App.Start((Device device) => {});
		}
	}
}