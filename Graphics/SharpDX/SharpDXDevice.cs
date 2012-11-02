using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using DriverType = SharpDX.Direct3D.DriverType;
using D2dFactory = SharpDX.Direct2D1.Factory;
using DxDevice = SharpDX.Direct3D11.Device;
using DxSwapChain = SharpDX.DXGI.SwapChain;
using Resource = SharpDX.Direct3D11.Resource;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;

namespace DeltaEngine.Graphics.SharpDX
{
	/// <summary>
	/// Provides DirectX 11 support with extended range of features like Geometry shader, Hardware
	/// tessellation and compute shaders. Currently just used to support SharpDXDrawing.
	/// </summary>
	public class SharpDXDevice : Device
	{
		public SharpDXDevice(Window window)
		{
			this.window = window;
			width = (int)window.ViewportSize.Width;
			height = (int)window.ViewportSize.Height;
			DxDevice.CreateWithSwapChain(DriverType.Hardware,
#if DEBUG
				DeviceCreationFlags.Debug |
#endif
				DeviceCreationFlags.BgraSupport,
				CreateSwapChainDescription(), out device, out swapChain);
			direct2DFactory = new D2dFactory();
			backBuffer = Resource.FromSwapChain<Texture2D>(swapChain, 0);
			surface = backBuffer.QueryInterface<Surface>();
			RenderTarget = new RenderTarget(direct2DFactory, surface, defaultRenderTargetProperties);
			window.ViewportSizeChanged += ResetDeviceToNewViewportSize;
			Screen = new ScreenSpace(window.ViewportSize);
		}

		private readonly Window window;
		private int width;
		private int height;
		private readonly DxDevice device;
		private Texture2D backBuffer;
		private readonly DxSwapChain swapChain;
		private readonly D2dFactory direct2DFactory;
		private Surface surface;
		internal RenderTarget RenderTarget { get; private set; }
		private readonly RenderTargetProperties defaultRenderTargetProperties =
			new RenderTargetProperties(new PixelFormat(Format.Unknown, AlphaMode.Premultiplied));

		private SwapChainDescription CreateSwapChainDescription()
		{
			return new SwapChainDescription
			{
				BufferCount = BackBufferCount,
				ModeDescription = new ModeDescription(width, height, new Rational(60, 1), BackBufferFormat),
				IsWindowed = true,
				OutputHandle = window.Handle,
				SampleDescription = new SampleDescription(1, 0),
				SwapEffect = SwapEffect.Discard,
				Usage = Usage.RenderTargetOutput
			};
		}

		private const int BackBufferCount = 1;
		private const Format BackBufferFormat = Format.R8G8B8A8_UNorm;

		private void ResetDeviceToNewViewportSize(Size newSizeInPixel)
		{
			backBuffer.Dispose();
			surface.Dispose();
			RenderTarget.Dispose();

			width = (int)newSizeInPixel.Width;
			height = (int)newSizeInPixel.Height;
			swapChain.ResizeBuffers(BackBufferCount, width, height, BackBufferFormat, SwapChainFlags.None);
			backBuffer = Resource.FromSwapChain<Texture2D>(swapChain, 0);
			surface = backBuffer.QueryInterface<Surface>();
			RenderTarget = new RenderTarget(direct2DFactory, surface, defaultRenderTargetProperties);
			Screen = new ScreenSpace(newSizeInPixel);
		}
		
		public ScreenSpace Screen { get; private set; }

		public void Run()
		{
			RenderTarget.BeginDraw();
			if (window.BackgroundColor.A > 0)
				RenderTarget.Clear(new Color4(window.BackgroundColor.PackedArgb));
		}

		public void Present()
		{
			RenderTarget.EndDraw();
			swapChain.Present(0, PresentFlags.None);
		}

		public void Dispose()
		{
			backBuffer.Dispose();
			swapChain.Dispose();
			surface.Dispose();
			RenderTarget.Dispose();
			direct2DFactory.Dispose();
			device.Dispose();
		}
	}
}