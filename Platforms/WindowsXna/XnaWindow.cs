using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using Microsoft.Xna.Framework;
using Color = DeltaEngine.Datatypes.Color;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Window support via the buildin XNA Game.Window functionality; supports fullscreen mode.
	/// </summary>
	public class XnaWindow : Window
	{
		public XnaWindow(Game game)
		{
			this.game = game;
			game.Window.Title = StackTraceExtensions.GetEntryName();
			game.Window.AllowUserResizing = true;
			game.IsMouseVisible = true;
			game.Window.ClientSizeChanged += OnViewportSizeChanged;
		}

		private readonly Game game;

		private void OnViewportSizeChanged(object sender, EventArgs e)
		{
			if (ViewportSizeChanged != null)
				ViewportSizeChanged(ViewportSize);
		}

		public string Title
		{
			get { return game.Window.Title; }
			set { game.Window.Title = value; }
		}

		public bool IsVisible
		{
			get { return game.IsActive; }
		}

		public IntPtr Handle
		{
			get { return game.Window.Handle; }
		}

		public Size ViewportSize
		{
			get { return new Size(game.Window.ClientBounds.Width, game.Window.ClientBounds.Height); }
		}

		public Size TotalSize
		{
			get { return new Size(game.Window.ClientBounds.Width, game.Window.ClientBounds.Height); }
			set
			{
				game.Window.BeginScreenDeviceChange(false);
				game.Window.EndScreenDeviceChange("", (int)value.Width, (int)value.Height);
			}
		}
		
		public Color BackgroundColor { get; set; }

		public bool IsFullscreen
		{
			get { return !game.Window.AllowUserResizing; }
			set
			{
				game.Window.AllowUserResizing = value;
				game.Window.BeginScreenDeviceChange(value);
				game.Window.EndScreenDeviceChange("");
			}
		}
		public bool IsClosing { get; private set; }

		public event Action<Size> ViewportSizeChanged;

		public void Run()
		{
			FrameworkDispatcher.Update();
		}

		public void Dispose()
		{
			if (IsClosing)
				return;

			IsClosing = true;
			FrameworkDispatcher.Update();
			game.Exit();
		}
	}
}