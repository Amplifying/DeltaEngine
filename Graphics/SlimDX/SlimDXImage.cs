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
		public SlimDXImage(string filename, Logger log, SlimDXDevice device)
			: base(filename)
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
				NativeTexture = Texture.FromFile(device.NativeDevice, filename, Usage.None, Pool.Managed);
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
			NativeTexture = new Texture(device.NativeDevice, (int)DefaultTextureSize.Width,
				(int)DefaultTextureSize.Height, 0, Usage.None, Format.A8B8G8R8, Pool.Managed);
			pixelSize = DefaultTextureSize;
		}

		public void Draw(VertexPositionColorTextured[] vertices)
		{
			device.NativeDevice.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.None);
			device.NativeDevice.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Linear);
			device.NativeDevice.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Linear);
			device.NativeDevice.SetTexture(0, NativeTexture);
		}

		public override Size PixelSize
		{
			get { return pixelSize; }
		}

		public override bool HasAlpha
		{
			get
			{
				//TODO
				return false;
			}
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