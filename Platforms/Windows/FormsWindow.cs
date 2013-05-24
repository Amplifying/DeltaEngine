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
				ClientSize = new System.Drawing.Size(640, 360),
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
			closeAfterOneFrameIfInIntegrationTest =
				!StackTraceExtensions.ContainsNoTestOrIsVisualTest();
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

		public bool Visibility
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
			set { ResizeCentered(value.Width, value.Height); }
		}

		private void ResizeCentered(float widthInPixels, float heightInPixels)
		{
			int xPosOffset = (int)((form.Width - widthInPixels) / 2.0f);
			int yPosOffset = (int)((form.Height - heightInPixels) / 2.0f);
			form.Location = new System.Drawing.Point(form.Location.X + xPosOffset,
				form.Location.Y + yPosOffset);
			form.Size = new System.Drawing.Size((int)widthInPixels, (int)heightInPixels);
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
			SetResolutionAndScreenMode(displaySize);
		}

		public void SetWindowed()
		{
			IsFullscreen = false;
			SetResolutionAndScreenMode(rememberedWindowedSize);
		}

		private Size rememberedWindowedSize;

		private void SetResolutionAndScreenMode(Size displaySize)
		{
			form.TopMost = true;
			form.StartPosition = FormStartPosition.Manual;
			form.DesktopLocation = new System.Drawing.Point(0, 0);
			form.FormBorderStyle = IsFullscreen ? FormBorderStyle.None : FormBorderStyle.Sizable;
			TotalPixelSize = displaySize;
			if (FullscreenChanged != null)
				FullscreenChanged(displaySize, IsFullscreen);
		}

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

		public MessageBoxButton ShowMessageBox(string title, string message, MessageBoxButton buttons)
		{
			var buttonCombination = MessageBoxButtons.OK;
			if ((buttons & MessageBoxButton.Cancel) != 0)
				buttonCombination = MessageBoxButtons.OKCancel;
			if ((buttons & MessageBoxButton.Ignore) != 0)
				buttonCombination = MessageBoxButtons.AbortRetryIgnore;
			var result = MessageBox.Show(form, message, Title + " " + title, buttonCombination);
			if (result == DialogResult.OK || result == DialogResult.Abort)
				return MessageBoxButton.Okay;
			return result == DialogResult.Ignore ? MessageBoxButton.Ignore : MessageBoxButton.Cancel;
		}

		private bool remDisabledShowCursor;

		public void Run()
		{
			Application.DoEvents();
		}

		public void Present()
		{
			if (closeAfterOneFrameIfInIntegrationTest)
				form.Close();

			if (IsClosing)
			{
				if (WindowClosing != null)
					WindowClosing();
			}
				
		}

		public event Action WindowClosing;

		public void Dispose()
		{
			form.Dispose();
		}
	}
}