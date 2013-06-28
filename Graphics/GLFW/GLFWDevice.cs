using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using System;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;

namespace DeltaEngine.Graphics.GLFW
{
	public sealed class GLFWDevice : Device
	{
		public GLFWDevice(Window window)
		{
			this.window = window;
			Initialize();
			window.ViewportSizeChanged += size => InitializeProjectionMatrix();
			window.FullscreenChanged += OnFullscreenChanged;
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		private readonly Window window;

		private void Initialize()
		{
			InitializeModelViewMatrix();
			InitializeProjectionMatrix();
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

		private static void OnFullscreenChanged(Size displaySize, bool isFullscreen)
		{
			throw new NotSupportedException();
		}

		public void Run()
		{
			var color = window.BackgroundColor;
			if (color.A <= 0)
				return;

			GL.ClearColor(color.RedValue, color.GreenValue, color.BlueValue, color.AlphaValue);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
		}

		public void Present()
		{
			Glfw.SwapBuffers();
		}

		public void Dispose()
		{
		}
	}
}