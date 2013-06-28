using System;
using System.IO;
using DeltaEngine.Datatypes;
using Microsoft.Xna.Framework.Graphics;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace DeltaEngine.Graphics.Xna
{
	/// <summary>
	/// Type of Image used for Xna-Textures. Provides methods for loading and displaying that are
	/// native to Xna and not useful for other frameworks.
	/// </summary>
	public class XnaImage : Image
	{
		public XnaImage(string contentName, XnaDevice device)
			: base(contentName)
		{
			nativeDevice = device.NativeDevice;
			if (nativeDevice == null || device.NativeContent == null)
				throw new UnableToContinueWithoutXnaGraphicsDevice();
		}

		private readonly GraphicsDevice nativeDevice;

		private class UnableToContinueWithoutXnaGraphicsDevice : Exception {}

		protected XnaImage(XnaDevice device, Texture2D nativeTexture)
			: base("<NativeImage>")
		{
			nativeDevice = device.NativeDevice;
			NativeTexture = nativeTexture;
		}

		protected override void LoadImage(Stream fileData)
		{
			NativeTexture = Texture2D.FromStream(nativeDevice, fileData);
		}

		public Texture2D NativeTexture { get; protected set; }

		protected override void DisposeData()
		{
			if (NativeTexture != null)
				NativeTexture.Dispose();
		}

		protected override void CreateDefaultTexture()
		{
			base.CreateDefaultTexture();
			NativeTexture = new Texture2D(nativeDevice, (int)DefaultTextureSize.Width,
				(int)DefaultTextureSize.Height);
			NativeTexture.SetData(ConvertToXnaColors(checkerMapColors));
		}

		protected override void SetSamplerState()
		{
			nativeDevice.Textures[0] = NativeTexture;
			nativeDevice.SamplerStates[0] = DisableLinearFiltering
				? SamplerState.PointClamp : SamplerState.LinearClamp;
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