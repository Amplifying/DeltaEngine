using System;
using System.Diagnostics;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Logging;

namespace DeltaEngine.Graphics
{
	/// <summary>
	/// Provides a way to load images. Use Drawing to show them on the screen.
	/// </summary>
	public abstract class Image : ContentData
	{
		protected Image(string contentName)
			: base(contentName) {}

		protected readonly Color[] checkerMapColors =
		{
			Color.LightGray, Color.DarkGray,
			Color.LightGray, Color.DarkGray, Color.DarkGray, Color.LightGray, Color.DarkGray,
			Color.LightGray, Color.LightGray, Color.DarkGray, Color.LightGray, Color.DarkGray,
			Color.DarkGray, Color.LightGray, Color.DarkGray, Color.LightGray
		};

		protected override void LoadData(Stream fileData)
		{
			ExtractMetaData();
			TryLoadImage(fileData);
			SetSamplerState();
		}

		private void ExtractMetaData()
		{
			PixelSize = MetaData.Get("PixelSize", DefaultTextureSize);
			BlendMode = MetaData.Get("BlendMode", BlendMode.Normal);
			UseMipmaps = MetaData.Get("UseMipmaps", false);
			AllowTiling = MetaData.Get("AllowTiling", false);
			DisableLinearFiltering = MetaData.Get("DisableLinearFiltering", false);
		}

		public Size PixelSize { get; private set; }
		protected static readonly Size DefaultTextureSize = new Size(4, 4);
		public BlendMode BlendMode { get; private set; }
		public bool UseMipmaps { get; private set; }
		public bool AllowTiling { get; private set; }
		public bool DisableLinearFiltering { get; private set; }

		private void TryLoadImage(Stream fileData)
		{
			try
			{
				LoadImage(fileData);
			}
			catch (Exception ex)
			{
				Logger.Current.Error(ex);
				if (!Debugger.IsAttached)
					CreateDefaultTexture();
				else
					throw;
			}
		}

		protected abstract void LoadImage(Stream fileData);

		protected virtual void CreateDefaultTexture()
		{
			PixelSize = DefaultTextureSize;
			DisableLinearFiltering = true;
			BlendMode = BlendMode.Opaque;
		}

		protected abstract void SetSamplerState();
	}
}