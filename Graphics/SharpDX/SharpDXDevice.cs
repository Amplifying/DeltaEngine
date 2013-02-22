using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using D2dFactory = SharpDX.Direct2D1.Factory;
using DxDevice = SharpDX.Direct3D11.Device;
using DxSwapChain = SharpDX.DXGI.SwapChain;
using CreationFlags = SharpDX.Direct3D11.DeviceCreationFlags;
using DeltaRect = DeltaEngine.Datatypes.Rectangle;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using Resource = SharpDX.Direct3D11.Resource;

namespace DeltaEngine.Graphics.SharpDX
{
	/// <summary>
	/// Provides DirectX 11 support with extended range of features like Geometry shader, Hardware
	/// tessellation and compute shaders. Currently just used to support SharpDXDrawing.
	/// </summary>
	public class SharpDXDevice : SharpDXStates, Device
	{
		public SharpDXDevice(Window window)
		{
			this.window = window;
			if (window.Title == "")
				window.Title = "SharpDX Device";

			DxDevice.CreateWithSwapChain(DriverType.Hardware, CreationFlags,
				CreateSwapChainDescription(Width, Height, window.Handle), out nativeDevice, out swapChain);
			window.ViewportSizeChanged += ResetDeviceToNewViewportSize;
			ResetDeviceToNewViewportSize(window.ViewportPixelSize);
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
		private readonly DxDevice nativeDevice;
		internal DxDevice NativeDevice
		{
			get { return nativeDevice; }
		}
		private readonly DxSwapChain swapChain;

		private void ResetDeviceToNewViewportSize(Size newSizeInPixel)
		{
			ResizeBackBufferIfItExistedBefore();
			backBuffer = Resource.FromSwapChain<Texture2D>(swapChain, 0);
			backBufferView = new RenderTargetView(nativeDevice, backBuffer);
			surface = backBuffer.QueryInterface<Surface>();
			RenderTarget = new RenderTarget(d2DFactory, surface, defaultRenderTargetProperties);
		}

		private void ResizeBackBufferIfItExistedBefore()
		{
			if (backBuffer == null)
				return;

			backBuffer.Dispose();
			backBufferView.Dispose();
			surface.Dispose();
			RenderTarget.Dispose();
			swapChain.ResizeBuffers(BackBufferCount, Width, Height, BackBufferFormat, BackBufferFlags);
		}

		public DeviceContext Context
		{
			get { return nativeDevice.ImmediateContext; }
		}
		private Texture2D backBuffer;
		private RenderTargetView backBufferView;
		private Surface surface;
		internal RenderTarget RenderTarget { get; private set; }

		public void Run()
		{
			if (nativeDevice.IsDisposed)
				return;

			RenderTarget.BeginDraw();
			Context.OutputMerger.SetTargets(backBufferView);
			Context.Rasterizer.SetViewports(new Viewport(0, 0, Width, Height, 0.0f, 1.0f));
			if (window.BackgroundColor.A > 0)
				Context.ClearRenderTargetView(backBufferView, new Color4(window.BackgroundColor.PackedRgba));
			Context.Rasterizer.State = CullClockwise(nativeDevice);
		}

		public void Present()
		{
			if (nativeDevice.IsDisposed)
				return;

			RenderTarget.EndDraw();
			swapChain.Present(0, PresentFlags.None);
		}

		public override void Dispose()
		{
			base.Dispose();
			backBuffer.Dispose();
			swapChain.Dispose();
			surface.Dispose();
			RenderTarget.Dispose();
			if (nativeDevice.IsDisposed == false)
			{
				nativeDevice.ImmediateContext.Dispose();
#if DEBUG
				// Helps finding any remaining unreleased references via console output, which is NOT empty,
				// but contains several Refcount: 0 lines. This cannot be avoided, but is still be useful to
				// find memory leaks (Refcount>0): http://sharpdx.org/forum/4-general/1241-reportliveobjects
				var deviceDebug = new DeviceDebug(nativeDevice);
				deviceDebug.ReportLiveDeviceObjects(ReportingLevel.Detail);
#endif
			}
			nativeDevice.Dispose();
		}

		public BlendState AlphaBlendState
		{
			get
			{
				return alphaBlend ??
					(alphaBlend = new BlendState(nativeDevice, GetBlendStateDescription()));
			}
		}

		private BlendState alphaBlend;

		public void SetData<T>(Buffer buffer, T[] data, int count = 0) where T : struct
		{
			DataStream dataStream;
			Context.MapSubresource(buffer, MapMode.WriteDiscard, MapFlags.None, out dataStream);
			dataStream.WriteRange(data, 0, count == 0 ? data.Length : count);
			Context.UnmapSubresource(buffer, 0);
		}
	}
}