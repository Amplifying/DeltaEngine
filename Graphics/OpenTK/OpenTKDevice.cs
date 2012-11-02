using System;
using System.Drawing;
using DeltaEngine.Platforms;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;

namespace DeltaEngine.Graphics.OpenTK
{
	/// <summary>
	/// OpenTK GL device implementation.
	/// </summary>
	public sealed class OpenTKDevice : Device
	{
		public OpenTKDevice(Window window)
		{
			this.window = window;
			window.Title = "OpenTK Device";
			InitGL();
			InitializeModelViewMatrix();
			InitializeProjectionMatrix();
			window.ViewportSizeChanged += size => InitializeProjectionMatrix();
		}

		private readonly Window window;
		private IWindowInfo windowInfo;
		private GraphicsContext context;

		public ScreenSpace Screen { get; private set; }

		public void Run()
		{
			var color = window.BackgroundColor;
			if (color.A <= 0)
				return;

			GL.ClearColor(Color.FromArgb(color.R, color.G, color.B));
			GL.Clear(ClearBufferMask.ColorBufferBit);
		}

		public void Present()
		{
			if (window.IsVisible && context != null)
				context.SwapBuffers();
		}

		public void Dispose()
		{
			if (context != null)
				context.Dispose();
			context = null;
		}

		private void InitGL()
		{
			windowInfo = Utilities.CreateWindowsWindowInfo(window.Handle);
			context = new GraphicsContext(GraphicsMode.Default, windowInfo);
			context.MakeCurrent(windowInfo);
			context.LoadAll();
		}

		private static void InitializeModelViewMatrix()
		{
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();
		}

		private void InitializeProjectionMatrix()
		{
			Screen = new ScreenSpace(window.ViewportSize);
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			var width = (int)window.ViewportSize.Width;
			var height = (int)window.ViewportSize.Height;
			GL.Ortho(0, width, height, 0, -1, 1);
			GL.Viewport(0, 0, width, height);
		}
	}
}