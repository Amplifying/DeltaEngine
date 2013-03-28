using System;
using DeltaEngine.Datatypes;
using Microsoft.Xna.Framework.Graphics;
using Color = DeltaEngine.Datatypes.Color;
using ContentManager = Microsoft.Xna.Framework.Content.ContentManager;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace DeltaEngine.Graphics.Xna
{
	public class XnaImage : Image
	{
		public XnaImage(string contentName, XnaDrawing drawing, XnaDevice device)
			: base(contentName, drawing)
		{
			nativeDevice = device.NativeDevice;
			if (nativeDevice == null || device.NativeContent == null)
				throw new UnableToContinueWithoutXnaGraphicsDevice();

			TryLoadImageData(contentName, device.NativeContent);
		}

		private readonly GraphicsDevice nativeDevice;

		internal class UnableToContinueWithoutXnaGraphicsDevice : Exception {}

		private void TryLoadImageData(string contentName, ContentManager nativeContent)
		{
			try
			{
				LoadImageData(contentName, nativeContent);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Failed to load texture '" + contentName + "': " + ex);
				CreateDefaultTexture();
			}
		}

		private void LoadImageData(string contentName, ContentManager nativeContent)
		{
			NativeTexture = nativeContent.Load<Texture2D>(contentName);
			pixelSize = new Size(NativeTexture.Width, NativeTexture.Height);
		}

		public Texture2D NativeTexture { get; private set; }
		private Size pixelSize;
		public override Size PixelSize { get { return pixelSize; } }

		public override void Dispose()
		{
			if (NativeTexture != null)
				NativeTexture.Dispose();
		}

		public override void Draw(VertexPositionColorTextured[] vertices)
		{
			nativeDevice.Textures[0] = NativeTexture;
			nativeDevice.SamplerStates[0] = DisableLinearFiltering ?
				SamplerState.PointClamp :
				SamplerState.LinearClamp;
			base.Draw(vertices);
		}

		private void CreateDefaultTexture()
		{
			NativeTexture = new Texture2D(nativeDevice, (int)DefaultTextureSize.Width,
				(int)DefaultTextureSize.Height);
			NativeTexture.SetData(ConvertToXnaColors(checkerMapColors));
			pixelSize = DefaultTextureSize;
			DisableLinearFiltering = true;
		}

		private static XnaColor[] ConvertToXnaColors(Color[] deltaColors)
		{
			var colors = new XnaColor[deltaColors.Length];
			for (int index = 0; index < deltaColors.Length; index++)
			{
				var color = deltaColors[index];
				colors[index] = new XnaColor(color.R, color.G, color.B, color.A);
			}
			return colors;
		}
	}
}