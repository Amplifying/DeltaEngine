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
			window.FullscreenChanged += OnFullscreenChanged;
			if (window.Title == "")
				window.Title = "XNA Device";
			CreateAndSetupNativeDeviceManager(game);
		}

		private readonly Window window;

		private void CreateAndSetupNativeDeviceManager(Game game)
		{
			game.SuppressDraw();
			CreateDeviceManager(game);
			NativeContent = game.Content;
			NativeContent.RootDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Content");
		}

		private void CreateDeviceManager(Game game)
		{
			deviceManager = new GraphicsDeviceManager(game)
			{
				SupportedOrientations =
					DisplayOrientation.Portrait | DisplayOrientation.LandscapeLeft |
						DisplayOrientation.LandscapeRight,
				SynchronizeWithVerticalRetrace = false,
				PreferredBackBufferFormat = SurfaceFormat.Color,
				PreferredBackBufferWidth = 640,
				PreferredBackBufferHeight = 360,
				GraphicsProfile = GraphicsProfile.HiDef
			};
			deviceManager.ApplyChanges();
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

		private void OnFullscreenChanged(Size displaySize, bool isFullScreenEnabled)
		{
			deviceManager.PreferredBackBufferWidth = (int)displaySize.Width;
			deviceManager.PreferredBackBufferHeight = (int)displaySize.Height;
			deviceManager.IsFullScreen = isFullScreenEnabled;
		}

		public ContentManager NativeContent { get; private set; }

		public void Run()
		{
			var color = window.BackgroundColor;
			if (color.A > 0)
				NativeDevice.Clear(new Color(color.R, color.G, color.B, color.A));
		}

		public void Present() {}

		public void Dispose() {}
	}
}