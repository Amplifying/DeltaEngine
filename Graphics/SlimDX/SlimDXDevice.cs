using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using SlimDX;
using SlimDX.DXGI;
using SlimDX.Direct2D;
using SlimDX.Direct3D11;
using Factory = SlimDX.Direct2D.Factory;
using SlimD3D11 = SlimDX.Direct3D11;

namespace DeltaEngine.Graphics.SlimDX
{
    public class SlimDXDevice : Device
    {
			public SlimDXDevice(Window window)
			{
				this.window = window;
				SlimD3D11.Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.BgraSupport,
					GetSwapChainDescription(), out device, out swapChain);
				window.ViewportSizeChanged += ResetDeviceToNewViewportSize;
				ResetDeviceToNewViewportSize(window.ViewportPixelSize);
				window.FullscreenChanged += OnFullscreenChanged;
			}

	    private readonly Window window;

			private int Width
			{
				get { return (int)window.ViewportPixelSize.Width; }
			}

			private int Height
			{
				get { return (int)window.ViewportPixelSize.Height; }
			}

	    private readonly SlimD3D11.Device device;
	    private readonly SwapChain swapChain;

			internal SlimD3D11.Device Device
			{
				get { return device; }
			}

			internal DeviceContext Context
			{
				get { return device.ImmediateContext; }
			}
			
			internal RenderTarget RenderTarget { get; private set; }

			private SwapChainDescription GetSwapChainDescription()
			{
				return new SwapChainDescription
				{
					BufferCount = 2,
					Usage = Usage.RenderTargetOutput,
					OutputHandle = window.Handle,
					IsWindowed = !window.IsFullscreen,
					ModeDescription = new ModeDescription(0, 0, new Rational(60, 1), Format.R8G8B8A8_UNorm),
					SampleDescription = new SampleDescription(1, 0),
					Flags = SwapChainFlags.AllowModeSwitch,
					SwapEffect = SwapEffect.Discard
				};
			}

			private void ResetDeviceToNewViewportSize(Size newSizeInPixel)
			{
				ResizeBackBufferIfItExistedBefore();
				backBuffer = SlimD3D11.Resource.FromSwapChain<Texture2D>(swapChain, 0);
				backBufferView = new RenderTargetView(device, backBuffer);
				var properties = new WindowRenderTargetProperties();
				properties.Handle = window.Handle;
				properties.PixelSize = new System.Drawing.Size((int)window.ViewportPixelSize.Width,
					(int)window.ViewportPixelSize.Height);
				RenderTarget = new WindowRenderTarget(new Factory(), properties);
			}

			private Texture2D backBuffer;
			private RenderTargetView backBufferView;

			private void ResizeBackBufferIfItExistedBefore()
			{
				if (backBuffer == null)
					return;

				backBuffer.Dispose();
				backBufferView.Dispose();
				RenderTarget.Dispose();
				swapChain.ResizeBuffers(1, Width, Height, Format.B8G8R8A8_UNorm,
					SwapChainFlags.AllowModeSwitch);
			}

			private void OnFullscreenChanged(Size displaySize, bool b)
			{
				if(device != null)
					device.Dispose();

				ResetDeviceToNewViewportSize(window.ViewportPixelSize);
			}

	    public void Run()
	    {
				RenderTarget.BeginDraw();
				Context.OutputMerger.SetTargets(backBufferView);
				var viewport = new Viewport(0.0f, 0.0f, window.ViewportPixelSize.Width,
					window.ViewportPixelSize.Height);
				Context.Rasterizer.SetViewports(viewport);
		    Context.ClearRenderTargetView(backBufferView,
			    new Color4(window.BackgroundColor.R, window.BackgroundColor.G, window.BackgroundColor.B));
	    }

			public void Present()
			{
				RenderTarget.EndDraw();
				swapChain.Present(0, PresentFlags.None);
			}

	    public void Dispose()
	    {
				backBuffer.Dispose();
				backBufferView.Dispose();
				RenderTarget.Dispose();
				swapChain.Dispose();
				device.Dispose();
	    }
    }
}
