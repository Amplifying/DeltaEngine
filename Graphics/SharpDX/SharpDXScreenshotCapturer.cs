using SharpDX.Direct3D11;
using DxDevice = SharpDX.Direct3D11.Device;

namespace DeltaEngine.Graphics.SharpDX
{
	public class SharpDXScreenshotCapturer : ScreenshotCapturer
	{
		public SharpDXScreenshotCapturer(Device device)
		{
			this.device = device;
		}

		private readonly Device device;

		public void MakeScreenshot(string fileName)
		{
			var d3DDevice = (SharpDXDevice)device;
			Resource.ToFile(d3DDevice.Context, d3DDevice.BackBuffer, ImageFileFormat.Png, fileName);
		}
	}
}