using System;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Platforms.Mocks
{
	public class MockWindow : Window
	{
		public MockWindow()
		{
			Title = "MockWindow";
			TotalPixelSize = new Size(640, 360);
		}

		public void Run() {}

		public void Present()
		{
			if (IsClosing && WindowClosing != null)
				WindowClosing();
		}

		public void CloseAfterFrame() {}
		public void Dispose() {}

		public string Title { get; set; }

		public bool Visibility
		{
			get { return true; }
		}

		public IntPtr Handle
		{
			get { return IntPtr.Zero; }
		}

		public Size ViewportPixelSize
		{
			get { return currentSize; }
			set
			{
				currentSize = value;
				if (ViewportSizeChanged != null)
					ViewportSizeChanged(currentSize);

				if (OrientationChanged != null)
					OrientationChanged(Orientation);
			}
		}

		public Orientation Orientation
		{
			get { return Orientation.Landscape; }
		}

		public event Action<Size> ViewportSizeChanged;
		public event Action<Orientation> OrientationChanged;
		public event Action<Size, bool> FullscreenChanged;

		public Size TotalPixelSize
		{
			get { return currentSize; }
			set
			{
				currentSize = value;
				if (ViewportSizeChanged == null)
					return;
				ViewportSizeChanged(currentSize);
			}
		}

		private Size currentSize;
		public Point PixelPosition { get; set; }
		public Color BackgroundColor { get; set; }
		public bool IsFullscreen { get; private set; }

		public void SetFullscreen(Size displaySize)
		{
			IsFullscreen = true;
			rememberSizeBeforeFullscreen = TotalPixelSize;
			TotalPixelSize = displaySize;
			if (FullscreenChanged != null)
				FullscreenChanged(TotalPixelSize, true);
		}

		private Size rememberSizeBeforeFullscreen;

		public void SetWindowed()
		{
			IsFullscreen = false;
			TotalPixelSize = rememberSizeBeforeFullscreen;
		}

		public bool IsClosing
		{
			get { return true; }
		}

		public bool ShowCursor { get; set; }

		public MessageBoxButton ShowMessageBox(string title, string message, MessageBoxButton buttons)
		{
			return MessageBoxButton.Okay;
		}

		public event Action WindowClosing;
	}
}