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
		public OpenTKImage(string contentName, OpenTKDevice device, Logger logger) : base(contentName)
		{
			if (device.Context == null)
				throw new UnableToLoadImageWithoutValidGraphicsDevice();

			this.logger = logger;
		}

		private readonly Logger logger;

		public override bool HasAlpha
		{
			get
			{
				return hasImageAlpha;
			}
		}

		private bool hasImageAlpha;
		private const int InvalidHandle = -1;
		private int glHandle = InvalidHandle;

		public int Handle
		{
			get
			{
				return glHandle;
			}
		}

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
			TryLoadBitmap(fileData);
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

		private void TryLoadBitmap(Stream fileData)
		{
			try
			{
				LoadBitmap(fileData);
			}
			catch (Exception ex)
			{
				logger.Error(ex);
				if (!Debugger.IsAttached)
					CreateDefaultTexture();
				else
					throw;
			}
		}

		private void LoadBitmap(Stream fileData)
		{
			using (var bitmap = new Bitmap(fileData))
			{
				LoadBitmapDataIntoTextureHandle(bitmap);
				size = new Size(bitmap.Width, bitmap.Height);
				hasImageAlpha = IsUsingAlphaChannel(bitmap);
			}
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

		private static bool IsUsingAlphaChannel(Bitmap bitmap)
		{
			for (int y = 0; y < bitmap.Height; y++)
				for (int x = 0; x < bitmap.Width; x++)
					if (bitmap.GetPixel(x, y).A != 255)
						return true;

			return false;
		}

		private void CreateDefaultTexture()
		{
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 
				(int)DefaultTextureSize.Width, (int)DefaultTextureSize.Height, 0, 
					global::OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, 
						BinaryDataExtensions.GetBytesFromArray(checkerMapColors));
			size = DefaultTextureSize;
			DisableLinearFiltering = true;
			hasImageAlpha = false;
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

		protected override void DisposeData()
		{
			if (glHandle == InvalidHandle)
				return;

			GL.DeleteTexture(glHandle);
			glHandle = InvalidHandle;
		}
		private class UnableToLoadImageWithoutValidGraphicsDevice : Exception
		{
		}
	}
}