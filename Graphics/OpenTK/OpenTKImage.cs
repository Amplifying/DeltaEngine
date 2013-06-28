using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using DeltaEngine.Datatypes;
using OpenTK.Graphics.OpenGL;
using Color = DeltaEngine.Datatypes.Color;
using GlPixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Rectangle = System.Drawing.Rectangle;
using Size = DeltaEngine.Datatypes.Size;

namespace DeltaEngine.Graphics.OpenTK
{
	public class OpenTKImage : Image
	{
		public OpenTKImage(string contentName, OpenTKDevice device) : base(contentName)
		{
			if (device.Context == null)
				throw new UnableToLoadImageWithoutValidGraphicsDevice();
		}
		
		private class UnableToLoadImageWithoutValidGraphicsDevice : Exception { }

		private const int InvalidHandle = -1;
		private int glHandle = InvalidHandle;

		public int Handle
		{
			get
			{
				return glHandle;
			}
		}

		protected override void LoadImage(Stream fileData)
		{
			using (var bitmap = new Bitmap(fileData))
			{
				LoadBitmapDataIntoTextureHandle(bitmap);
				var bitmapSize = new Size(bitmap.Width, bitmap.Height);
				if (bitmapSize != PixelSize)
					throw new InvalidPixelSizeFromMetaData("Bitmap size: " + bitmapSize + " is different " +
						"form MetaData PixelSize: " + PixelSize);
			}
		}

		private void LoadBitmapDataIntoTextureHandle(Bitmap bitmap)
		{
			InitializeTextureHandle();
			var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), 
				ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, 
				data.Height, 0, GlPixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
			bitmap.UnlockBits(data);
		}

		private void InitializeTextureHandle()
		{
			GL.GenTextures(1, out glHandle);
			GL.BindTexture(TextureTarget.Texture2D, glHandle);
		}

		protected override void CreateDefaultTexture()
		{
			base.CreateDefaultTexture();
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 
				(int)DefaultTextureSize.Width, (int)DefaultTextureSize.Height, 0, GlPixelFormat.Bgra, 
					PixelType.UnsignedByte, Color.GetBytesFromArray(checkerMapColors));
		}

		protected override void SetSamplerState()
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
		private class InvalidPixelSizeFromMetaData : Exception
		{
			public InvalidPixelSizeFromMetaData(string message) : base(message)
			{
			}
		}
	}
}