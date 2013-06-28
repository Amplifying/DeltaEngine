using System.IO;
using SlimDX.Direct3D9;

namespace DeltaEngine.Graphics.SlimDX
{
	public class SlimDXImage : Image
	{
		public SlimDXImage(string contentName, SlimDXDevice device)
			: base(contentName)
		{
			this.device = device;
		}

		private readonly SlimDXDevice device;

		protected override void LoadImage(Stream fileData)
		{
			NativeTexture = Texture.FromStream(device.NativeDevice, fileData, Usage.None, Pool.Managed);
		}

		public Texture NativeTexture { get; private set; }

		protected override void CreateDefaultTexture()
		{
			base.CreateDefaultTexture();
			NativeTexture = new Texture(device.NativeDevice, (int)DefaultTextureSize.Width,
				(int)DefaultTextureSize.Height, 0, Usage.None, Format.A8B8G8R8, Pool.Managed);
		}

		protected override void SetSamplerState()
		{
			device.NativeDevice.SetTexture(0, NativeTexture);
			device.NativeDevice.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.None);
			device.NativeDevice.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Linear);
			device.NativeDevice.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Linear);
		}

		protected override void DisposeData()
		{
			NativeTexture.Dispose();
		}
	}
}