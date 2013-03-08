using System;
using System.Drawing;
using System.Windows.Forms;
using DeltaEngine.Core;
using Color = DeltaEngine.Datatypes.Color;
using Point = DeltaEngine.Datatypes.Point;
using Size = DeltaEngine.Datatypes.Size;

namespace DeltaEngine.Platforms.Windows
{
	/// <summary>
	/// Implementation of the Windows Forms window for the Delta Engine to run game loops.
	/// </summary>
	public class FormsWindow : Window
	{
		public FormsWindow()
		{
			form = new NativeMessageForm
			{
				Text = StackTraceExtensions.GetEntryName(),
				Size = new System.Drawing.Size(1024, 640),
				MinimumSize = new System.Drawing.Size(1, 1),
				FormBorderStyle = FormBorderStyle.Sizable,
				StartPosition = FormStartPosition.CenterScreen
			};
			BackgroundColor = Color.Black;
			Icon appIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
			if (appIcon != null)
				form.Icon = appIcon;
			form.SizeChanged += OnSizeChanged;
			form.Show();
			closeAfterOneFrameIfInIntegrationTest = !StackTraceExtensions.ContainsNoTestOrIsVisualTest();
		}

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

		private readonly Form form;
		private readonly bool closeAfterOneFrameIfInIntegrationTest;

		public event NativeMessageDelegate NativeEvent
		{
			add
			{
				var nativeMessageForm = form as NativeMessageForm;
				if (nativeMessageForm != null)
					nativeMessageForm.NativeEvent += value;
			}
			remove
			{
				var nativeMessageForm = form as NativeMessageForm;
				if (nativeMessageForm != null)
					nativeMessageForm.NativeEvent -= value;
			}
		}

		public delegate void NativeMessageDelegate(ref Message m);

		private void OnSizeChanged(object sender, EventArgs e)
		{
			if (ViewportSizeChanged != null)
				ViewportSizeChanged(ViewportPixelSize);
			if (OrientationChanged != null)
				OrientationChanged(ViewportPixelSize.Width > ViewportPixelSize.Height
					? Orientation.Landscape : Orientation.Portrait);
		}

		public event Action<Size> ViewportSizeChanged;
		public event Action<Orientation> OrientationChanged;

		public string Title
		{
			get { return form.Text; }
			set { form.Text = value; }
		}

		public bool IsVisible
		{
			get { return form.Visible; }
		}

		public IntPtr Handle
		{
			get { return form.IsDisposed ? IntPtr.Zero : form.Handle; }
		}

		public Size ViewportPixelSize
		{
			get { return new Size(form.ClientSize.Width, form.ClientSize.Height); }
		}

		public Size TotalPixelSize
		{
			get { return new Size(form.Width, form.Height); }
			set { form.Size = new System.Drawing.Size((int)value.Width, (int)value.Height); }
		}

		public Point PixelPosition
		{
			get { return new Point(form.Location.X, form.Location.Y); }
		}

		public Color BackgroundColor
		{
			get { return color; }
			set
			{
				color = value;
				if (color.A > 0)
					form.BackColor = System.Drawing.Color.FromArgb(color.R, color.G, color.B);
			}
		}
		private Color color;
		
		public void SetFullscreen(Size displaySize)
		{
			IsFullscreen = true;
			rememberedWindowedSize = new Size(form.Size.Width, form.Size.Height);
			form.TopMost = true;
			form.StartPosition = FormStartPosition.Manual;
			form.DesktopLocation = new System.Drawing.Point(0, 0);
			form.FormBorderStyle = FormBorderStyle.None;
			TotalPixelSize = displaySize;
			if (FullscreenChanged != null)
				FullscreenChanged(displaySize, IsFullscreen);
		}

		public void SetWindowed()
		{
			IsFullscreen = false;
			form.TopMost = true;
			form.StartPosition = FormStartPosition.Manual;
			form.DesktopLocation = new System.Drawing.Point(0, 0);
			form.FormBorderStyle = FormBorderStyle.Sizable;
			TotalPixelSize = rememberedWindowedSize;
			if (FullscreenChanged != null)
				FullscreenChanged(rememberedWindowedSize, IsFullscreen);
		}

		private Size rememberedWindowedSize;

		public bool IsFullscreen { get; private set; }

		public event Action<Size, bool> FullscreenChanged;

		public bool IsClosing
		{
			get { return form.Disposing || form.IsDisposed; }
		}

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
		private bool remDisabledShowCursor;

		public void Run()
		{
			Application.DoEvents();
			if (closeAfterOneFrameIfInIntegrationTest)
				form.Close();
		}

		public void Dispose()
		{
			form.Dispose();
		}
	}
}