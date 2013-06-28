using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using Color = System.Drawing.Color;

namespace DeltaEngine.Graphics.OpenTK
{
	public sealed class OpenTKDevice : Device
	{
		public OpenTKDevice(Window window)
		{
			this.window = window;
			Initialize();
			window.ViewportSizeChanged += size => InitializeProjectionMatrix();
			window.FullscreenChanged += OnFullscreenChanged;
		}

		private IWindowInfo windowInfo;

		public GraphicsContext Context
		{
			get;
			private set;
		}

		private const int BitsPerPixel = 32;
		private readonly Window window;

		private void Initialize()
		{
			windowInfo = Utilities.CreateWindowsWindowInfo(window.Handle);
			CreateContext();
			InitializeModelViewMatrix();
			InitializeProjectionMatrix();
		}

		private void CreateContext()
		{
			Context = new GraphicsContext(GraphicsMode.Default, windowInfo);
			Context.MakeCurrent(windowInfo);
			Context.LoadAll();
		}

		private void InitializeModelViewMatrix()
		{
			SetModelViewMatrix(Matrix.Identity);
		}

		public void SetModelViewMatrix(Matrix matrix)
		{
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadMatrix(matrix.GetValues);
		}

		private void InitializeProjectionMatrix()
		{
			var projection = Matrix.GenerateOrthographicProjection(window.ViewportPixelSize);
			SetProjectionMatrix(projection);
			GL.Viewport(0, 0, (int)window.ViewportPixelSize.Width, (int)window.ViewportPixelSize.Height);
		}

		public void SetProjectionMatrix(Matrix matrix)
		{
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadMatrix(matrix.GetValues);
		}

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
			if (window.Visibility && Context != null)
				Context.SwapBuffers();
		}

		public void Dispose()
		{
			if (windowInfo != null)
				windowInfo.Dispose();

			if (Context != null)
				Context.Dispose();

			Context = null;
		}

		private static void OnFullscreenChanged(Size displaySize, bool b)
		{
			DisplayResolution resolution = 
				DisplayDevice.Default.SelectResolution((int)displaySize.Width, (int)displaySize.Height, 
					BitsPerPixel, 0);
			DisplayDevice.Default.ChangeResolution(resolution);
			if (IsResolutionNotEqualDisplaySize(resolution, displaySize))
				throw new ResolutionRequestFailed("Could not find resolution: " + displaySize);
		}

		private static bool IsResolutionNotEqualDisplaySize(DisplayResolution resolution, Size 
			displaySize)
		{
			return resolution.Width != (int)displaySize.Width || resolution.Height != 
				(int)displaySize.Height;
		}
		private class ResolutionRequestFailed : Exception
		{
			public ResolutionRequestFailed(string message) : base(message)
			{
			}
		}
	}
}