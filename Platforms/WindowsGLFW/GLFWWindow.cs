using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DeltaEngine.Core;
using Pencil.Gaming;
using Color = DeltaEngine.Datatypes.Color;
using Point = DeltaEngine.Datatypes.Point;
using Size = DeltaEngine.Datatypes.Size;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// GLFW window implementation for the Delta Engine to run applications in in.
	/// </summary>
	public class GLFWWindow : Window
	{
		public GLFWWindow(Settings settings)
		{
			if (!Glfw.Init())
				throw new UnableToInitializeGLFW();

			this.settings = settings;
			CreateWindow(settings.Resolution, settings.StartInFullscreen);
			Title = StackTraceExtensions.GetEntryName();
			BackgroundColor = Color.Black;
			Glfw.SetWindowSizeCallback(OnWindowResize);
			hwnd = GetForegroundWindow();
			Icon appIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
			if (appIcon != null && hwnd != IntPtr.Zero)
				SetIcon(appIcon);
		}

		private readonly Settings settings;

		public class UnableToInitializeGLFW : Exception {}
		public class UnableToCreateGLFWWindow : Exception {}

		private void CreateWindow(Size resolution, bool startInFullscreen)
		{
			viewportSize = totalSize = resolution;
			var width = (int)resolution.Width;
			var height = (int)resolution.Height;
			Glfw.OpenWindowHint(OpenWindowHint.FSAASamples, settings.AntiAliasingSamples);
			OpenWindow(startInFullscreen, width, height);

			if (!startInFullscreen)
				CenterOnScreen(width, height);
		}

		private void OpenWindow(bool startInFullscreen, int width, int height)
		{
			var colorBits = GetColorBits();
			if (Glfw.OpenWindow(width, height, colorBits[0], colorBits[1], colorBits[2], 0,
				settings.DepthBufferBits, 0, startInFullscreen ? WindowMode.FullScreen : WindowMode.Window) == 0)
				throw new UnableToCreateGLFWWindow();
		}

		private int[] GetColorBits()
		{
			if (settings.ColorBufferBits >= 24)
				return new[] { 8, 8, 8 };
			if (settings.ColorBufferBits >= 16)
				return new[] { 5, 6, 5 };

			throw new UnsupportedFramebuffeFormat();
		}

		public class UnsupportedFramebuffeFormat : Exception { }

		private void CenterOnScreen(int width, int height)
		{
			GlfwVidMode desktop;
			Glfw.GetDesktopMode(out desktop);
			PixelPosition = new Point(desktop.Width / 2 - width / 2, desktop.Height / 2 - height / 2);
		}

		public string Title
		{
			get { return title; }
			set
			{
				title = value;
				Glfw.SetWindowTitle(title);
			}
		}

		private string title;

		private void OnWindowResize(int width, int height)
		{
			if (width == 0 || height == 0)
				return;
			viewportSize = totalSize = new Size(width, height);
			Orientation = width > height ? Orientation.Landscape : Orientation.Portrait;
			if (ViewportSizeChanged != null)
				ViewportSizeChanged(ViewportPixelSize);
			if (OrientationChanged != null)
				OrientationChanged(Orientation);
		}

		public Orientation Orientation { get; private set; }

		public event Action<Size> ViewportSizeChanged;
		public event Action<Orientation> OrientationChanged;

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		private static extern int DrawMenuBar(int currentWindow);

		private const int WMSeticon = 0x80;
		private const int IconSmall = 0;
		private readonly IntPtr hwnd;

		private void SetIcon(Icon icon)
		{
			SendMessage(hwnd, WMSeticon, (IntPtr)IconSmall, icon.Handle);
			DrawMenuBar((int)hwnd);
		}

		public bool Visibility
		{
			get { return true; }
		}

		public IntPtr Handle
		{
			get { throw new NotSupportedException(); }
		}

		public Size ViewportPixelSize
		{
			get { return viewportSize; }
			set
			{
				viewportSize = totalSize = value;
				Glfw.SetWindowSize((int)value.Width, (int)value.Height);
			}
		}

		private Size viewportSize;

		public Size TotalPixelSize
		{
			get { return totalSize; }
		}
		private Size totalSize;

		public Point PixelPosition
		{
			get { return position; }
			set
			{
				position = value;
				Glfw.SetWindowPos((int)value.X, (int)value.Y);
			}
		}

		private Point position;

		public Color BackgroundColor { get; set; }

		public virtual void SetFullscreen(Size setFullscreenViewportSize)
		{
			if (FullscreenChanged != null)
				FullscreenChanged(setFullscreenViewportSize, true);

			throw new NotSupportedException();
		}

		public void SetWindowed()
		{
			throw new NotSupportedException();
		}

		public bool IsFullscreen
		{
			get { return false; }
		}
		public event Action<Size, bool> FullscreenChanged;

		public virtual bool IsClosing
		{
			get { return Glfw.GetWindowParam(WindowParam.Opened) == 0 || rememberToClose; }
		}
		private bool rememberToClose;

		public bool ShowCursor
		{
			get { return !remDisabledShowCursor; }
			set
			{
				if (remDisabledShowCursor != value)
					return;

				remDisabledShowCursor = !value;
				if (remDisabledShowCursor)
					Cursor.Hide();
				else
					Cursor.Show();
			}
		}

		public MessageBoxButton ShowMessageBox(string caption, string message,
			MessageBoxButton buttons)
		{
			var buttonCombination = MessageBoxButtons.OK;
			if ((buttons & MessageBoxButton.Cancel) != 0)
				buttonCombination = MessageBoxButtons.OKCancel;
			if ((buttons & MessageBoxButton.Ignore) != 0)
				buttonCombination = MessageBoxButtons.AbortRetryIgnore;
			var result = MessageBox.Show(message, Title + " " + caption, buttonCombination);
			if (result == DialogResult.OK || result == DialogResult.Abort)
				return MessageBoxButton.Okay;
			return result == DialogResult.Ignore ? MessageBoxButton.Ignore : MessageBoxButton.Cancel;
		}

		private bool remDisabledShowCursor;

		public virtual void Run() {}

		public void Present()
		{
			Glfw.PollEvents();
			AllowAltF4ToCloseWindow();
			if (IsClosing && WindowClosing != null)
			{
				WindowClosing();
				WindowClosing = null;
			}
		}

		private void AllowAltF4ToCloseWindow()
		{
			if (Glfw.GetKey(Key.LeftAlt) && Glfw.GetKey(Key.F4))
				CloseAfterFrame();
		}

		public event Action WindowClosing;

		public void CloseAfterFrame()
		{
			rememberToClose = true;
		}

		public void Dispose()
		{
			CloseAfterFrame();
			Glfw.CloseWindow();
			if (WindowClosing != null)
				WindowClosing();
			WindowClosing = null;
			Glfw.Terminate();
		}
	}
}