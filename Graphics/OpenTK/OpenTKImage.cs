using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using Size = DeltaEngine.Datatypes.Size;
using SystemPixelFormat = System.Drawing.Imaging.PixelFormat;

namespace DeltaEngine.Graphics.OpenTK
{
	/// <summary>
	/// Facilitates draw images on screen. Needs a drawing to show it.
	/// </summary>
	public class OpenTKImage : Image
	{
		public OpenTKImage(string filename, Drawing drawing)
			: base(filename, drawing)
		{
			InitializeTextureHandle();
			TryLoadBitmapFile("Content/" + filename + ".png");
			SetSamplerState();
		}

		private const int InvalidHandle = -1;

		public override void Dispose()
		{
			if (glHandle == InvalidHandle)
				return;

			GL.DeleteTexture(glHandle);
			glHandle = InvalidHandle;
		}

		private void InitializeTextureHandle()
		{
			GL.GenTextures(1, out glHandle);
			GL.BindTexture(TextureTarget.Texture2D, glHandle);
		}

		private int glHandle = InvalidHandle;

		private void TryLoadBitmapFile(string filename)
		{
			try
			{
				LoadBitmapFile(filename);
			}
			catch (ArgumentException ex)
			{
				Console.WriteLine("Failed to load texture, file '" + filename + "' does not exist: " + ex);
				CreateDefaultTexture();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Failed to load texture '" + filename + "': " + ex);
				CreateDefaultTexture();
			}
		}

		private void LoadBitmapFile(string filename)
		{
			using (var bitmap = new Bitmap(filename))
			{
				LoadBitmapDataIntoTextureHandle(bitmap);
				pixelSize = new Size(bitmap.Width, bitmap.Height);
			}
		}

		private Size pixelSize;
		public override Size PixelSize { get { return pixelSize; } }

		private static void LoadBitmapDataIntoTextureHandle(Bitmap bitmap)
		{
			var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
				ImageLockMode.ReadOnly, SystemPixelFormat.Format32bppArgb);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height,
				0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
			bitmap.UnlockBits(data);
		}

		private void CreateDefaultTexture()
		{
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
				(int)DefaultTextureSize.Width, (int)DefaultTextureSize.Height, 0, PixelFormat.Bgra,
				PixelType.UnsignedByte, Datatypes.Color.GetBytes(checkerMapColors));
			pixelSize = DefaultTextureSize;
			DisableLinearFiltering = true;
		}

		private void SetSamplerState()
		{
			SetTexture2DParameter(TextureParameterName.TextureMinFilter,
				DisableLinearFiltering ? All.Nearest : All.Linear);
			SetTexture2DParameter(TextureParameterName.TextureMagFilter,
				DisableLinearFiltering ? All.Nearest : All.Linear);
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