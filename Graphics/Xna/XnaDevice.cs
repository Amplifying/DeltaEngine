using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace DeltaEngine.Graphics.Xna
{
	public class XnaDevice : Device
	{
		public XnaDevice(Window window, Game game)
		{
			this.window = window;
			this.game = game;
			window.ViewportSizeChanged += ResetDeviceToNewViewportSize;
			Screen = new ScreenSpace(window.ViewportSize);
			CreateDeviceManager();
		}

		private readonly Window window;
		private readonly Game game;
		public ScreenSpace Screen { get; private set; }

		private void ResetDeviceToNewViewportSize(Size newSizeInPixel)
		{
			Screen = new ScreenSpace(newSizeInPixel);
			NativeDevice.PresentationParameters.BackBufferWidth = (int)newSizeInPixel.Width;
			NativeDevice.PresentationParameters.BackBufferHeight = (int)newSizeInPixel.Height;
			NativeDevice.Reset(NativeDevice.PresentationParameters);
		}

		private void CreateDeviceManager()
		{
			deviceManager = new GraphicsDeviceManager(game)
			{
				SupportedOrientations = DisplayOrientation.Default,
				IsFullScreen = false,
				PreferredBackBufferWidth = (int)window.ViewportSize.Width,
				PreferredBackBufferHeight = (int)window.ViewportSize.Height,
				GraphicsProfile = GraphicsProfile.HiDef,
				SynchronizeWithVerticalRetrace = false,
			};
		}

		private GraphicsDeviceManager deviceManager;
		public GraphicsDevice NativeDevice
		{
			get { return deviceManager.GraphicsDevice; }
		}

		public void Run()
		{
			if (NativeDevice == null)
				return;

			var color = window.BackgroundColor;
			if (color.A > 0)
				NativeDevice.Clear(new Color(color.R, color.G, color.B));
		}

		public void Present()
		{
			// NativeDevice.Present is called by EndDraw at the end of the RunFrame inside Game.Tick
		}

		public void Dispose() {}
	}
}