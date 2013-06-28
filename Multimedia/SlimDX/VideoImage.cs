using System;
using System.IO;
using DeltaEngine.Graphics.SlimDX;
using DeltaEngine.Multimedia.AviVideo;
using DeltaEngine.Platforms;
using SlimDX;
using SlimDX.DXGI;
using SlimDX.Direct3D10;

namespace DeltaEngine.Multimedia.SlimDX
{
	[IgnoreForResolver]
	public class VideoImage : SlimDXImage
	{
		public VideoImage(SlimDXDevice device)
			: base("<VideoTexture>", device)
		{
			this.device = device;
		}

		private readonly SlimDXDevice device;

		protected override void LoadData(Stream fileData) {}

		public //TODO unsafe 
			void UpdateTexture(VideoStream stream, int frameIndex)
		{
			/*TODO:
			vertices[0].TextureCoordinate = Point.UnitY;
			vertices[1].TextureCoordinate = Point.One;
			vertices[2].TextureCoordinate = Point.UnitX;
			vertices[3].TextureCoordinate = Point.Zero;
			 *
			AviInterop.BitmapInfoHeader bitmapHeader;
			byte[] pixelData = stream.GetStreamData(frameIndex, out bitmapHeader);
			pixelData = ConvertToRgba(pixelData);
			fixed (byte* ptr = &pixelData[0])
				NativeTexture = new Texture2D(device.NativeDevice, CreateTextureDescription(bitmapHeader),
					new DataRectangle((IntPtr)ptr, bitmapHeader.biWidth * 4));
			NativeResourceView = new ShaderResourceView(device.NativeDevice, NativeTexture);
			 */
		}

		private static byte[] ConvertToRgba(byte[] pixelData)
		{
			byte[] result = new byte[(pixelData.Length / 3) * 4];
			for (int i = 0, destIndex = 0; i < pixelData.Length; i += 3, destIndex += 4)
			{
				result[destIndex] = pixelData[i + 2];
				result[destIndex + 1] = pixelData[i + 1];
				result[destIndex + 2] = pixelData[i];
				result[destIndex + 3] = 255;
			}
			return result;
		}

		private static Texture2DDescription CreateTextureDescription(
			AviInterop.BitmapInfoHeader bitmapHeader)
		{
			return new Texture2DDescription
			{
				Width = bitmapHeader.biWidth,
				Height = bitmapHeader.biHeight,
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
	}
}