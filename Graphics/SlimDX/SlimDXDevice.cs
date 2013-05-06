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
			SetAlphaBlending();
			window.ViewportSizeChanged += OnViewportSizeChanged;
			window.FullscreenChanged += OnFullscreenChanged;
		}

		private readonly Window window;
		private readonly Direct3D d3D;

		public SlimD3D9.Device Device { get; private set; }

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
			
			Device = new SlimD3D9.Device(d3D, 0, DeviceType.Hardware, window.Handle,
				CreateFlags.HardwareVertexProcessing, presentParameters);
		}
		
		private void SetAlphaBlending()
		{
			Device.SetRenderState(RenderState.AlphaBlendEnable, true);
			Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
			Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
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
			if (Device == null)
				return;

			if (deviceMustBeReset)
				ResetDevice();

			var slimDxColor = new Color4(window.BackgroundColor.AlphaValue,
				window.BackgroundColor.RedValue, window.BackgroundColor.GreenValue,
				window.BackgroundColor.BlueValue);
			Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, slimDxColor, 1.0f, 0);
			Device.BeginScene();
			runExecuted = true;
		}

		private bool runExecuted;

		private void ResetDevice()
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

			deviceMustBeReset = false;
		}

		public void Present()
		{
			if (Device == null || !runExecuted)
				return;

			Device.EndScene();
			Device.Present();
		}

		public void Dispose()
		{
			Device.Dispose();
			d3D.Dispose();
		}
	}
}