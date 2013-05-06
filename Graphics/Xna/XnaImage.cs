using System;
using System.Diagnostics;
using System.IO;
using DeltaEngine.Datatypes;
using DeltaEngine.Logging;
using Microsoft.Xna.Framework.Content;
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
		public XnaImage(string contentName, Drawing drawing, XnaDevice device, Logger logger,
			ContentManager manager)
			: base(contentName, drawing)
		{
			nativeDevice = device.NativeDevice;
			this.logger = logger;
			contentManager = manager;
			if (nativeDevice == null || device.NativeContent == null)
				throw new UnableToContinueWithoutXnaGraphicsDevice();
		}

		private readonly GraphicsDevice nativeDevice;
		private readonly ContentManager contentManager;
		private readonly Logger logger;

		private class UnableToContinueWithoutXnaGraphicsDevice : Exception {}

		protected XnaImage(Drawing drawing, XnaDevice device, Texture2D nativeTexture)
			: base("<NativeImage>", drawing)
		{
			nativeDevice = device.NativeDevice;
			NativeTexture = nativeTexture;
		}

		protected override void LoadData(Stream fileData)
		{
			NativeTexture = Texture2D.FromStream(nativeDevice, fileData);
			pixelSize = new Size(NativeTexture.Width, NativeTexture.Height);
		}

		protected override bool CanLoadDataFromStream
		{
			get { return false; }
		}

		protected override void LoadFromContentName(string contentName)
		{
			try
			{
				LoadImageData(contentName);
			}
			catch (Exception ex)
			{
				logger.Error(ex);
				if (!Debugger.IsAttached)
					CreateDefaultTexture();
				else
					throw new XnaTextureContentNotFound(contentName, ex);
			}
		}

		private void LoadImageData(string contentName)
		{
			NativeTexture = contentManager.Load<Texture2D>(contentName);
			pixelSize = new Size(NativeTexture.Width, NativeTexture.Height);
		}

		public Texture2D NativeTexture { get; protected set; }
		private Size pixelSize;
		public override Size PixelSize
		{
			get { return pixelSize; }
		}

		protected override void DisposeData()
		{
			if (NativeTexture != null)
				NativeTexture.Dispose();
		}

		public override void Draw(VertexPositionColorTextured[] vertices)
		{
			nativeDevice.Textures[0] = NativeTexture;
			nativeDevice.SamplerStates[0] = DisableLinearFiltering
				? SamplerState.PointClamp : SamplerState.LinearClamp;
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

		public class XnaTextureContentNotFound : Exception
		{
			public XnaTextureContentNotFound(string contentName, Exception innerException)
				: base(contentName, innerException) { }
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