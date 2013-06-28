using System;
using System.Drawing;
using System.Windows.Forms;
using DeltaEngine.Core;
using Color = DeltaEngine.Datatypes.Color;
using Point = DeltaEngine.Datatypes.Point;
using Size = DeltaEngine.Datatypes.Size;
using SystemSize = System.Drawing.Size;

namespace DeltaEngine.Platforms.Windows
{
	/// <summary>
	/// Windows Forms window implementation for the Delta Engine to run applications in it.
	/// </summary>
	public class FormsWindow : Window
	{
		public FormsWindow(Settings settings)
			: this(new NativeMessageForm())
		{
			var form = panel as Form;
			if (settings.StartInFullscreen)
			{
				IsFullscreen = settings.StartInFullscreen;
				form.FormBorderStyle = FormBorderStyle.None;
				form.TopMost = true;
				form.StartPosition = FormStartPosition.Manual;
				form.DesktopLocation = new System.Drawing.Point(0, 0);
			}
			else
			{
				form.FormBorderStyle = FormBorderStyle.Sizable;
				form.StartPosition = FormStartPosition.CenterScreen;
			}
			form.ClientSize = new SystemSize((int)settings.Resolution.Width,
				(int)settings.Resolution.Height);
			form.MinimumSize = new SystemSize(1, 1);
			form.Text = StackTraceExtensions.GetEntryName();
			BackgroundColor = Color.Black;
			Icon appIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
			if (appIcon != null)
				form.Icon = appIcon;
			form.SizeChanged += OnSizeChanged;
			form.Show();
		}

		public FormsWindow(Control panel)
		{
			this.panel = panel;
		}

		protected Control panel;

		private sealed class NativeMessageForm : Form
		{
			protected override void WndProc(ref Message m)
			{
				if (NativeEvent != null)
					NativeEvent(ref m);

				base.WndProc(ref m);
			}

			public event NativeMessageDelegate NativeEvent;
		}

		public event NativeMessageDelegate NativeEvent
		{
			add
			{
				var nativeMessageForm = panel as NativeMessageForm;
				if (nativeMessageForm != null)
					nativeMessageForm.NativeEvent += value;
			}
			remove
			{
				var nativeMessageForm = panel as NativeMessageForm;
				if (nativeMessageForm != null)
					nativeMessageForm.NativeEvent -= value;
			}
		}

		public delegate void NativeMessageDelegate(ref Message m);

		protected void OnSizeChanged(object sender, EventArgs e)
		{
			if (ViewportSizeChanged != null)
				ViewportSizeChanged(ViewportPixelSize);
			Orientation = ViewportPixelSize.Width > ViewportPixelSize.Height
				? Orientation.Landscape : Orientation.Portrait;
			if (OrientationChanged != null)
				OrientationChanged(Orientation);
		}

		public Orientation Orientation { get; private set; }
		public event Action<Size> ViewportSizeChanged;
		public event Action<Orientation> OrientationChanged;

		public string Title
		{
			get { return panel.Text; }
			set { panel.Text = value; }
		}

		public bool Visibility
		{
			get { return panel.Visible; }
		}

		public IntPtr Handle
		{
			get { return panel.IsDisposed ? IntPtr.Zero : panel.Handle; }
		}

		public Size ViewportPixelSize
		{
			get { return new Size(panel.ClientSize.Width, panel.ClientSize.Height); }
			set { TotalPixelSize = value + (TotalPixelSize - ViewportPixelSize); }
		}

		public Size TotalPixelSize
		{
			get { return new Size(panel.Width, panel.Height); }
			set { ResizeCentered(value.Width, value.Height); }
		}

		protected virtual void ResizeCentered(float widthInPixels, float heightInPixels)
		{
			var xPosOffset = (int)((panel.Width - widthInPixels) / 2.0f);
			var yPosOffset = (int)((panel.Height - heightInPixels) / 2.0f);
			panel.Location = new System.Drawing.Point(panel.Location.X + xPosOffset,
				panel.Location.Y + yPosOffset);
			panel.Size = new System.Drawing.Size((int)widthInPixels, (int)heightInPixels);
		}

		public Point PixelPosition
		{
			get { return new Point(panel.Location.X, panel.Location.Y); }
			set { panel.Location = new System.Drawing.Point((int)value.X, (int)value.Y); }
		}

		public Color BackgroundColor
		{
			get { return color; }
			set
			{
				color = value;
				if (color.A > 0)
					panel.BackColor = System.Drawing.Color.FromArgb(color.R, color.G, color.B);
			}
		}
		private Color color;

		public virtual void SetFullscreen(Size setFullscreenViewportSize)
		{
			IsFullscreen = true;
			rememberedWindowedSize = new Size(panel.Size.Width, panel.Size.Height);
			var form = panel as Form;
			if (form != null)
			{
				form.TopMost = true;
				form.StartPosition = FormStartPosition.Manual;
				form.DesktopLocation = new System.Drawing.Point(0, 0);
			}
			SetResolution(setFullscreenViewportSize);
		}

		public void SetWindowed()
		{
			IsFullscreen = false;
			var form = panel as Form;
			if (form != null)
				form.FormBorderStyle = FormBorderStyle.Sizable;
			SetResolution(rememberedWindowedSize);
		}

		private Size rememberedWindowedSize;

		private void SetResolution(Size displaySize)
		{
			TotalPixelSize = displaySize;
			if (FullscreenChanged != null)
				FullscreenChanged(displaySize, IsFullscreen);
		}

		public bool IsFullscreen { get; private set; }
		public event Action<Size, bool> FullscreenChanged;

		public virtual bool IsClosing
		{
			get { return panel.Disposing || panel.IsDisposed || rememberToClose; }
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

		public MessageBoxButton ShowMessageBox(string title, string message, MessageBoxButton buttons)
		{
			var buttonCombination = MessageBoxButtons.OK;
			if ((buttons & MessageBoxButton.Cancel) != 0)
				buttonCombination = MessageBoxButtons.OKCancel;
			if ((buttons & MessageBoxButton.Ignore) != 0)
				buttonCombination = MessageBoxButtons.AbortRetryIgnore;
			var result = MessageBox.Show(panel, message, Title + " " + title, buttonCombination);
			if (result == DialogResult.OK || result == DialogResult.Abort)
				return MessageBoxButton.Okay;
			return result == DialogResult.Ignore ? MessageBoxButton.Ignore : MessageBoxButton.Cancel;
		}

		private bool remDisabledShowCursor;

		public virtual void Run()
		{
			Application.DoEvents();
		}

		public void Present()
		{
			if (IsClosing && WindowClosing != null)
				WindowClosing();
		}

		public event Action WindowClosing;

		public void CloseAfterFrame()
		{
			rememberToClose = true;
		}

		public virtual void Dispose()
		{
			var form = panel as Form;
			if (form != null)
				form.Close();
			panel.Dispose();
		}
	}
}