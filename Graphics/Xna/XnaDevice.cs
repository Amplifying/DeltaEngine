using System.IO;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace DeltaEngine.Graphics.Xna
{
	public class XnaDevice : Device
	{
		public XnaDevice(Game game, Window window)
		{
			this.window = window;
			window.ViewportSizeChanged += ResetDeviceToNewViewportSize;
			if (window.Title == "")
				window.Title = "XNA Device";
			CreateAndSetupNativeDeviceManager(game);
		}

		private readonly Window window;

		private void CreateAndSetupNativeDeviceManager(Game game)
		{
			game.SuppressDraw();
			deviceManager = new GraphicsDeviceManager(game);
			deviceManager.SupportedOrientations = DisplayOrientation.Portrait |
				DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
			deviceManager.SynchronizeWithVerticalRetrace = false;
			deviceManager.PreferredBackBufferFormat = SurfaceFormat.Color;
			NativeContent = game.Content;
			// We only need the 'Directory.GetCurrentDirectory()' for testing because otherwise Resharper
			// would fail (NCrunch and normal execution would work without too)
			NativeContent.RootDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Content");
		}

		private GraphicsDeviceManager deviceManager;
		public GraphicsDevice NativeDevice
		{
			get { return deviceManager.GraphicsDevice; }
		}

		private void ResetDeviceToNewViewportSize(Size newSizeInPixel)
		{
			NativeDevice.PresentationParameters.BackBufferWidth = (int)newSizeInPixel.Width;
			NativeDevice.PresentationParameters.BackBufferHeight = (int)newSizeInPixel.Height;
			NativeDevice.Reset(NativeDevice.PresentationParameters);
			NativeDevice.Clear(new Color(0, 0, 0));
		}

		public ContentManager NativeContent { get; private set; }

		public void Run()
		{
			var color = window.BackgroundColor;
			if (color.A > 0)
				NativeDevice.Clear(new Color(color.R, color.G, color.B));
		}

		public void Present() {}

		public void Dispose() {}
	}
}