using System;
using System.Drawing;
using System.Windows.Forms;
using DeltaEngine.Core;
using Color = DeltaEngine.Datatypes.Color;

namespace DeltaEngine.Platforms.Windows
{
	/// <summary>
	/// Implementation of the Windows Forms window for the Delta Engine to run game loops.
	/// </summary>
	public class FormsWindow : Window
	{
		public FormsWindow()
		{
			form = new Form
			{
				Text = StackTraceExtensions.GetEntryName(),
				Size = new Size(800, 600),
				FormBorderStyle = FormBorderStyle.Sizable,
				StartPosition = FormStartPosition.CenterScreen
			};
			Icon appIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
			if (appIcon != null)
				form.Icon = appIcon;
			form.SizeChanged += OnSizeChanged;
			form.Show();
			closeAfterOneFrameIfInIntegrationTest = !StackTraceExtensions.ContainsNoTestOrIsVisualTest();
		}

		private readonly Form form;
		private readonly bool closeAfterOneFrameIfInIntegrationTest;

		private void OnSizeChanged(object sender, EventArgs e)
		{
			if (ViewportSizeChanged != null)
				ViewportSizeChanged(ViewportSize);
		}

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
			get { return form.Handle; }
		}

		public Datatypes.Size ViewportSize
		{
			get { return new Datatypes.Size(form.ClientSize.Width, form.ClientSize.Height); }
		}

		public Datatypes.Size TotalSize
		{
			get { return new Datatypes.Size(form.Width, form.Height); }
			set { form.Size = new Size((int)value.Width, (int)value.Height); }
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

		public bool IsFullscreen
		{
			get { return isFullscreen; }
			set
			{
				isFullscreen = value;
				form.TopMost = true;
				form.StartPosition = FormStartPosition.Manual;
				form.DesktopLocation = new Point(0, 0);
			}
		}
		private bool isFullscreen;

		public bool IsClosing
		{
			get { return form.Disposing || form.IsDisposed; }
		}

		public event Action<Datatypes.Size> ViewportSizeChanged;

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