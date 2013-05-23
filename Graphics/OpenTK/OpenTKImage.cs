using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Size = DeltaEngine.Datatypes.Size;
using System.Drawing.Imaging;
using DeltaEngine.Datatypes;
using DeltaEngine.Logging;
using OpenTK.Graphics.OpenGL;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Rectangle = System.Drawing.Rectangle;

namespace DeltaEngine.Graphics.OpenTK
{
	public class OpenTKImage : Image
	{
		public OpenTKImage(string contentName, Drawing drawing, Logger logger) : base(contentName, 
			drawing)
		{
			this.logger = logger;
		}

		private readonly Logger logger;
		private const int InvalidHandle = -1;
		private int glHandle = InvalidHandle;
		private Size size;

		public override Size PixelSize
		{
			get
			{
				return size;
			}
		}

		private void TryLoadBitmapFile(string filename)
		{
			try
			{
				LoadBitmapFile(filename);
			}
			catch (Exception ex)
			{
				ExecuteLogger(ex);
				if (!Debugger.IsAttached)
					CreateDefaultTexture();
				else
					throw;
			}
		}

		private void LoadBitmapFile(string filename)
		{
			using (var bitmap = new Bitmap(filename))
			{
				LoadBitmapDataIntoTextureHandle(bitmap);
				size = new Size(bitmap.Width, bitmap.Height);
			}
		}

		protected override void LoadData(Stream fileData)
		{
			InitializeTextureHandle();
			using (var bitmap = new Bitmap(fileData))
			{
				LoadBitmapDataIntoTextureHandle(bitmap);
				size = new Size(bitmap.Width, bitmap.Height);
			}
			SetSamplerState();
		}

		private void LoadImageAndSetState(string file)
		{
			InitializeTextureHandle();
			TryLoadBitmapFile(file);
			SetSamplerState();
		}

		private void InitializeTextureHandle()
		{
			GL.GenTextures(1, out glHandle);
			GL.BindTexture(TextureTarget.Texture2D, glHandle);
		}

		private void ExecuteLogger(Exception ex)
		{
			logger.Error(ex);
		}

		protected override void DisposeData()
		{
			if (glHandle == InvalidHandle)
				return;

			GL.DeleteTexture(glHandle);
			glHandle = InvalidHandle;
		}

		private void LoadBitmapDataIntoTextureHandle(Bitmap bitmap)
		{
			var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), 
				ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, 
				data.Height, 0, global::OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, 
					data.Scan0);
			bitmap.UnlockBits(data);
		}

		private void CreateDefaultTexture()
		{
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 
				(int)DefaultTextureSize.Width, (int)DefaultTextureSize.Height, 0, 
					global::OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, 
						BinaryDataExtensions.GetBytesFromArray(checkerMapColors));
			size = DefaultTextureSize;
			DisableLinearFiltering = true;
		}

		private void SetSamplerState()
		{
			SetTexture2DParameter(TextureParameterName.TextureMinFilter, DisableLinearFiltering ? 
				All.Nearest : All.Linear);
			SetTexture2DParameter(TextureParameterName.TextureMagFilter, DisableLinearFiltering ? 
				All.Nearest : All.Linear);
			SetTexture2DParameter(TextureParameterName.TextureWrapS, All.ClampToEdge);
			SetTexture2DParameter(TextureParameterName.TextureWrapT, All.ClampToEdge);
		}

		private static void SetTexture2DParameter(TextureParameterName name, All value)
		{
			GL.TexParameter(TextureTarget.Texture2D, name, (int)value);
		}

		public override void Draw(VertexPositionColorTextured[] vertices)
		{
			GL.Enable(EnableCap.Texture2D);
			GL.BindTexture(TextureTarget.Texture2D, glHandle);
			base.Draw(vertices);
		}
	}
}