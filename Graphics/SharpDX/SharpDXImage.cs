using System;
using System.Diagnostics;
using System.IO;
using DeltaEngine.Datatypes;
using DeltaEngine.Logging;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct3D11;
using SharpDX.WIC;
using DxDevice = SharpDX.Direct3D11.Device;

namespace DeltaEngine.Graphics.SharpDX
{
	public class SharpDXImage : Image
	{
		public SharpDXImage(string filename, SharpDXDevice device, Logger log)
			: base(filename)
		{
			this.device = device;
			this.log = log;
		}

		private readonly SharpDXDevice device;
		private readonly Logger log;

		protected override void LoadData(Stream fileData)
		{
			TryLoadTexture("Content/" + Name + ".png");
		}

		protected override void DisposeData()
		{
			if (NativeTexture != null)
				NativeTexture.Dispose();

			if (NativeResourceView != null)
				NativeResourceView.Dispose();
		}

		private void TryLoadTexture(string filename)
		{
			try
			{
				NativeTexture = LoadTexture(filename);
				pixelSize = new Size(NativeTexture.Description.Width, NativeTexture.Description.Height);
				hasImageAlpha = true;//TODO: find out from meta data
			}
			catch (Exception ex)
			{
				log.Error(ex);
				if (!Debugger.IsAttached)
					CreateDefaultTexture();
				else
					throw;
			}

			NativeResourceView = new ShaderResourceView(device.NativeDevice, NativeTexture);
		}

		public Texture2D NativeTexture { get; protected set; }
		private Size pixelSize;
		public override Size PixelSize
		{
			get { return pixelSize; }
		}
		public override bool HasAlpha
		{
			get { return hasImageAlpha; }
		}
		private bool hasImageAlpha;
		public ShaderResourceView NativeResourceView { get; protected set; }

		public Texture2D LoadTexture(string filename)
		{
			using (var factory = new ImagingFactory())
			using (var originalSource = Decode(factory, filename))
			using (var bitmapSource = Convert(factory, originalSource))
				return CreateDirectXTexture(bitmapSource);
		}

		private static BitmapSource Decode(ImagingFactory factory, string filename)
		{
			using (var bitmapDecoder = new BitmapDecoder(factory, filename, DecodeOptions.CacheOnDemand)
				)
				return bitmapDecoder.GetFrame(0);
		}

		private static FormatConverter Convert(ImagingFactory factory, BitmapSource source)
		{
			var formatConverter = new FormatConverter(factory);
			formatConverter.Initialize(source, PixelFormat.Format32bppPRGBA, BitmapDitherType.None, null,
				0.0, BitmapPaletteType.Custom);
			return formatConverter;
		}

		private Texture2D CreateDirectXTexture(BitmapSource source)
		{
			var stride = source.Size.Width * 4;
			using (var buffer = GetPixels(source, stride))
				return new Texture2D(device.NativeDevice, CreateTextureDescription(source.Size),
					new DataRectangle(buffer.DataPointer, stride));
		}

		private static DataStream GetPixels(BitmapSource source, int stride)
		{
			var pixels = new DataStream(source.Size.Height * stride, true, true);
			source.CopyPixels(stride, pixels);
			return pixels;
		}

		private static Texture2DDescription CreateTextureDescription(DrawingSize size)
		{
			return new Texture2DDescription
			{
				Width = size.Width,
				Height = size.Height,
				ArraySize = 1,
				BindFlags = BindFlags.ShaderResource,
				Usage = ResourceUsage.Immutable,
				CpuAccessFlags = CpuAccessFlags.None,
				Format = Format.R8G8B8A8_UNorm,
				MipLevels = 1,
				OptionFlags = ResourceOptionFlags.None,
				SampleDescription = new SampleDescription(1, 0),
			};
		}

		private void CreateDefaultTexture()
		{
			Utilities.Pin(checkerMapColors,
				ptr =>
				{
					NativeTexture = new Texture2D(device.NativeDevice,
						new Texture2DDescription
						{
							Width = (int)DefaultTextureSize.Width,
							Height = (int)DefaultTextureSize.Height,
							ArraySize = 1,
							MipLevels = 1,
							Format = Format.R8G8B8A8_UNorm,
							Usage = ResourceUsage.Immutable,
							BindFlags = BindFlags.ShaderResource,
							SampleDescription = new SampleDescription(1, 0),
						}, new DataRectangle(ptr, 64));
				});
			pixelSize = DefaultTextureSize;
			DisableLinearFiltering = true;
			hasImageAlpha = false;
		}
	}
}