using SlimDX.Direct3D9;

namespace DeltaEngine.Graphics.SlimDX
{
	public class SlimDXScreenshotCapturer : ScreenshotCapturer
	{
		public SlimDXScreenshotCapturer(SlimDXDevice device)
		{
			this.device = device;
		}

		private readonly SlimDXDevice device;

		public void MakeScreenshot(string fileName)
		{
			Surface.ToFile(device.Device.GetBackBuffer(0, 0), fileName, ImageFileFormat.Png);
		}
	}
}