using DeltaEngine.Platforms;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using DxDevice = SharpDX.Direct3D11.Device;

namespace DeltaEngine.Graphics.SharpDX
{
	public class SharpDxScreenshotCapturer : ScreenshotCapturer
	{
		public SharpDxScreenshotCapturer(Device device, Window window)
		{
			this.window = window;
			this.device = device;
		}

		private readonly Device device;
		private readonly Window window;

		public void MakeScreenshot(string fileName)
		{
			SharpDXDevice d3DDevice = (SharpDXDevice)device;
			Resource.ToFile(d3DDevice.Context, d3DDevice.BackBuffer, ImageFileFormat.Png, fileName);
		}
	}
}