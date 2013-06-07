using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using SlimDX;
using SlimDX.Direct3D9;
using SlimD3D9 = SlimDX.Direct3D9;

namespace DeltaEngine.Graphics.SlimDX
{
	public class SlimDXDevice : Device
	{
		public SlimDXDevice(Window window)
		{
			this.window = window;
			d3D = new Direct3D();
			InitializeDevice();
			window.ViewportSizeChanged += OnViewportSizeChanged;
			window.FullscreenChanged += OnFullscreenChanged;
		}

		private readonly Window window;
		private readonly Direct3D d3D;

		public SlimD3D9.Device NativeDevice { get; private set; }

		private void InitializeDevice()
		{
			var presentParameters = new PresentParameters
			{
				Windowed = !window.IsFullscreen,
				DeviceWindowHandle = window.Handle,
				SwapEffect = SwapEffect.Discard,
				PresentationInterval = PresentInterval.Immediate,
				BackBufferWidth = (int)window.ViewportPixelSize.Width,
				BackBufferHeight = (int)window.ViewportPixelSize.Height
			};
			
			NativeDevice = new SlimD3D9.Device(d3D, 0, DeviceType.Hardware, window.Handle,
				CreateFlags.HardwareVertexProcessing, presentParameters);
		}

		private void OnViewportSizeChanged(Size displaySize)
		{
			deviceMustBeReset = true;
		}

		private bool deviceMustBeReset;

		private void OnFullscreenChanged(Size displaySize, bool b)
		{
			deviceMustBeReset = true;
		}

		public void Run()
		{
			if (NativeDevice == null)
				return;

			if (deviceMustBeReset)
				ResetDevice();

			ClearScreenAndBeginScene();
			runExecuted = true;
		}

		private bool runExecuted;

		private void ClearScreenAndBeginScene()
		{
			var slimDxColor = new Color4(window.BackgroundColor.AlphaValue,
				window.BackgroundColor.RedValue, window.BackgroundColor.GreenValue,
				window.BackgroundColor.BlueValue);
			NativeDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, slimDxColor, 1.0f, 0);
			NativeDevice.BeginScene();
		}

		private void ResetDevice()
		{
			DisposeResources();
			var presentParameters = new PresentParameters
			{
				Windowed = !window.IsFullscreen,
				DeviceWindowHandle = window.Handle,
				SwapEffect = SwapEffect.Discard,
				PresentationInterval = PresentInterval.Immediate,
				BackBufferCount = 1,
				BackBufferFormat = Format.A8R8G8B8,
				BackBufferWidth = (int)window.ViewportPixelSize.Width,
				BackBufferHeight = (int)window.ViewportPixelSize.Height
			};

			NativeDevice.Reset(presentParameters);
			deviceMustBeReset = false;
			if (OnDeviceReset != null)
				OnDeviceReset();
		}

		private void DisposeResources()
		{
			if (OnLostDevice != null)
				OnLostDevice();

			NativeDevice.GetRenderTarget(0).Dispose();
		}

		public void Present()
		{
			if (NativeDevice == null || !runExecuted)
				return;

			NativeDevice.EndScene();
			NativeDevice.Present();
		}

		public void Dispose()
		{
			NativeDevice.GetRenderTarget(0).Dispose();
			NativeDevice.Dispose();
			d3D.Dispose();
		}

		public event Action OnLostDevice;
		public event Action OnDeviceReset;
	}
}