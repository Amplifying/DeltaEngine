using System;
using System.IO;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using Pencil.Gaming.Graphics;

namespace DeltaEngine.Graphics.GLFW
{
	public class GLFWImage : Image
	{
		public GLFWImage(string contentName, GLFWDevice device) : base(contentName)
		{
			if (!device.IsInitialized)
				throw new UnableToLoadImageWithoutValidGraphicsDevice();
		}

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
			glHandle = GL.Utils.LoadImage(fileData);
		}

		protected override void CreateDefaultTexture()
		{
			base.CreateDefaultTexture();
			GL.GenTextures(1, out glHandle);
			GL.BindTexture(TextureTarget.Texture2D, Handle);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 
				(int)DefaultTextureSize.Width, (int)DefaultTextureSize.Height, 0, PixelFormat.Bgra, 
					PixelType.UnsignedByte, Color.GetBytesFromArray(checkerMapColors));
		}

		protected override void SetSamplerState()
		{
			GL.BindTexture(TextureTarget.Texture2D, Handle);
			SetTexture2DParameter(TextureParameterName.TextureMinFilter, (int)(DisableLinearFiltering ? 
				TextureMinFilter.Nearest : TextureMinFilter.Linear));
			SetTexture2DParameter(TextureParameterName.TextureMagFilter, (int)(DisableLinearFiltering ? 
				TextureMagFilter.Nearest : TextureMagFilter.Linear));
			SetTexture2DParameter(TextureParameterName.TextureWrapS, 
				(int)TextureParameterName.ClampToEdge);
			SetTexture2DParameter(TextureParameterName.TextureWrapT, 
				(int)TextureParameterName.ClampToEdge);
		}

		private static void SetTexture2DParameter(TextureParameterName name, int value)
		{
			GL.TexParameter(TextureTarget.Texture2D, name, value);
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