using System;
using DeltaEngine.Datatypes;
using SlimDX;
using SlimDX.DXGI;
using SlimDX.Direct3D11;

namespace DeltaEngine.Graphics.SlimDX
{
	public class SlimDXImage : Image
	{
		public SlimDXImage(string filename, SlimDXDrawing drawing, SlimDXDevice device)
			: base(filename, drawing)
		{
			this.device = device;
			SetupSamplerDescription();
			TryLoadTexture(@"Content\" + filename + ".png");
		}

		private readonly SlimDXDevice device;
		private Size pixelSize;

		public Texture2D NativeTexture { get; private set; }
		public ShaderResourceView NativeResourceView { get; private set; }

		private void SetupSamplerDescription()
		{
			samplerDescription = new SamplerDescription();
			samplerDescription.AddressU = TextureAddressMode.Wrap;
			samplerDescription.AddressV = TextureAddressMode.Wrap;
			samplerDescription.AddressW = TextureAddressMode.Wrap;
			samplerDescription.Filter = Filter.ComparisonMinPointMagMipLinear;			
		}

		private SamplerDescription samplerDescription;

		private void TryLoadTexture(string filename)
		{
			try
			{
				NativeTexture = Texture2D.FromFile(device.Device, filename);
				pixelSize = new Size(NativeTexture.Description.Width, NativeTexture.Description.Height);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Failed to load texture '" + filename + "': " + ex);
				CreateDefaultTexture();
			}

			NativeResourceView = new ShaderResourceView(device.Device, NativeTexture);
		}

		private void CreateDefaultTexture()
		{
			var description = new Texture2DDescription
				{
					Width = (int)DefaultTextureSize.Width,
					Height = (int)DefaultTextureSize.Height,
					ArraySize = 1,
					MipLevels = 1,
					Format = Format.R8G8B8A8_UNorm,
					Usage = ResourceUsage.Immutable,
					BindFlags = BindFlags.ShaderResource,
					SampleDescription = new SampleDescription(1, 0),
				};
			NativeTexture = new Texture2D(device.Device, description, new DataRectangle(64, new DataStream(64, true, true)));
			pixelSize = DefaultTextureSize;
			DisableLinearFiltering = true;
		}

		public override void Draw(VertexPositionColorTextured[] vertices)
		{
			device.Context.PixelShader.SetShaderResource(NativeResourceView, 0);
			base.Draw(vertices);
		}

		public override Size PixelSize
		{
			get { return pixelSize; }
		}

		public override void Dispose()
		{
			NativeTexture.Dispose();
		}
	}
}
