using System;
using System.Diagnostics;
using System.IO;
using DeltaEngine.Datatypes;
using DeltaEngine.Logging;
using SlimDX.Direct3D9;

namespace DeltaEngine.Graphics.SlimDX
{
	public class SlimDXImage : Image
	{
		public SlimDXImage(string filename, SlimDXDrawing drawing, Logger log, SlimDXDevice device)
			: base(filename, drawing)
		{
			this.device = device;
			this.log = log;
		}

		private readonly SlimDXDevice device;
		private readonly Logger log;
		private Size pixelSize;

		public Texture NativeTexture { get; private set; }

		private void TryLoadTexture(string filename)
		{
			try
			{
				NativeTexture = Texture.FromFile(device.Device, filename);
				pixelSize = new Size(NativeTexture.GetLevelDescription(0).Width,
					NativeTexture.GetLevelDescription(0).Height);
			}
			catch (Exception ex)
			{
				log.Error(ex);
				if (!Debugger.IsAttached)
					CreateDefaultTexture();
				else
					throw;
			}
		}

		private void CreateDefaultTexture()
		{
			NativeTexture = new Texture(device.Device, (int)DefaultTextureSize.Width,
				(int)DefaultTextureSize.Height, 0, Usage.None, Format.A8B8G8R8, Pool.Default);
			pixelSize = DefaultTextureSize;
		}

		public override void Draw(VertexPositionColorTextured[] vertices)
		{
			device.Device.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.None);
			device.Device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Anisotropic);
			device.Device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Anisotropic);
			device.Device.SetTexture(0, NativeTexture);
			base.Draw(vertices);
		}

		public override Size PixelSize
		{
			get { return pixelSize; }
		}

		protected override void LoadData(Stream fileData)
		{
			TryLoadTexture(@"Content\" + Name + ".png");
		}

		protected override void DisposeData()
		{
			NativeTexture.Dispose();
		}
	}
}