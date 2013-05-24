using System.IO;
using DeltaEngine.Platforms;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace DeltaEngine.Graphics.Xna
{
	public class XnaScreenshotCapturer : ScreenshotCapturer
	{
		public XnaScreenshotCapturer(Device device, Window window)
		{
			this.window = window;
			this.device = device;
		}

		private readonly Device device;
		private readonly Window window;

		public void MakeScreenshot(string fileName)
		{
			XnaDevice xnaDevice = (XnaDevice)device;
			var width = (int)window.ViewportPixelSize.Width;
			var height = (int)window.ViewportPixelSize.Height;
			using (var dstTexture = new Texture2D(xnaDevice.NativeDevice, width, height, false,
					xnaDevice.NativeDevice.PresentationParameters.BackBufferFormat))
			{
				var pixelColors = new Color[width * height];
				xnaDevice.NativeDevice.GetBackBufferData(pixelColors);
				dstTexture.SetData(pixelColors);

				using (var stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write,
						FileShare.ReadWrite))
					dstTexture.SaveAsPng(stream, width, height);
			}
		}
	}
}