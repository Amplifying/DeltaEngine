using System.IO;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct3D11;
using Resource = SharpDX.Direct3D11.Resource;

namespace DeltaEngine.Graphics.SharpDX
{
	public class SharpDXImage : Image
	{
		public SharpDXImage(string contentName, SharpDXDevice device)
			: base(contentName)
		{
			this.device = device;
		}

		private readonly SharpDXDevice device;

		protected override void LoadImage(Stream fileData)
		{
			NativeTexture =
				(Texture2D)Resource.FromStream(device.NativeDevice, fileData, (int)fileData.Length);
		}

		public Texture2D NativeTexture { get; protected set; }

		protected override void CreateDefaultTexture()
		{
			base.CreateDefaultTexture();
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
		}

		protected override void SetSamplerState()
		{
			NativeResourceView = new ShaderResourceView(device.NativeDevice, NativeTexture);
		}

		public ShaderResourceView NativeResourceView { get; protected set; }

		protected override void DisposeData()
		{
			if (NativeTexture != null)
				NativeTexture.Dispose();

			if (NativeResourceView != null)
				NativeResourceView.Dispose();
		}
	}
}