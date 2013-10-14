using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using DeltaEngine.Editor.Core;
using GalaSoft.MvvmLight.Messaging;
using Image = System.Drawing.Image;
using MenuItem = System.Windows.Forms.MenuItem;
using Panel = System.Windows.Forms.Panel;

namespace DeltaEngine.Editor.Emulator
{
	/// <summary>
	/// Engine Viewport for all of the Editor plugins
	/// </summary>
	public partial class ViewportControl : EditorPluginView
	{
		public ViewportControl()
		{
			InitializeComponent();
		}

		public void Init(Service service) {}

		public void ProjectChanged() {}

		public void ApplyEmulator()
		{
			SetupScreen();
			SetupDeviceFrame();
			SetupEmulator();
			SetupHost();
			/*TODO
			LoadDeviceInfoList();
			SetupMenu();
			LoadStateFromRegistry();
		 */
			Update();
		}

		private void SetupScreen()
		{
			Screen = new Panel();
			Screen.Dock = DockStyle.None;
		}

		public Panel Screen { get; private set; }

		private void SetupDeviceFrame()
		{
			deviceImage = new PictureBox();
			deviceImage.BackColor = Color.Transparent;
			deviceImage.BackgroundImageLayout = ImageLayout.Center;
			deviceImage.Location = new Point(0, 0);
			deviceImage.SizeMode = PictureBoxSizeMode.StretchImage;
		}

		private PictureBox deviceImage;

		private void SetupEmulator()
		{
			outerPanelForEmulator = new Panel();
			outerPanelForEmulator.Controls.Add(Screen);
			outerPanelForEmulator.Controls.Add(deviceImage);
			outerPanelForEmulator.Dock = DockStyle.Fill;
			outerPanelForEmulator.Controls.Add(deviceImage);
		}

		private Panel outerPanelForEmulator;

		private void SetupHost()
		{
			ViewportHost.Child = outerPanelForEmulator;
			((ISupportInitialize)(deviceImage)).EndInit();
			outerPanelForEmulator.ResumeLayout(false);
		}

		private void DragImage(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && !isDragging)
			{
				Messenger.Default.Send(true, "SetDraggingImage");
				isDragging = true;
				Mouse.OverrideCursor = System.Windows.Input.Cursors.Hand;
			}

			if (e.LeftButton != MouseButtonState.Pressed)
			{
				Messenger.Default.Send(false, "SetDraggingImage");
				isDragging = false;
				Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
			}
		}

		private bool isDragging;

		private void DragButton(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && !isDragging)
			{
				Messenger.Default.Send(true, "SetDraggingButton");
				isDragging = true;
				Mouse.OverrideCursor = System.Windows.Input.Cursors.Hand;
			}

			if (e.LeftButton != MouseButtonState.Pressed)
			{
				Messenger.Default.Send(false, "SetDraggingButton");
				isDragging = false;
				Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
			}
		}

		private void DragLabel(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && !isDragging)
			{
				Messenger.Default.Send(true, "SetDraggingLabel");
				isDragging = true;
				Mouse.OverrideCursor = System.Windows.Input.Cursors.Hand;
			}

			if (e.LeftButton != MouseButtonState.Pressed)
			{
				Messenger.Default.Send(false, "SetDraggingLabel");
				isDragging = false;
				Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
			}
		}

		private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

		private void Update()
		{
			if (!menuOrientation.Visible)
				SelectOrientation(0);

			if (!menuScale.Visible)
				SelectScale(0);

			//if (devices[selectedDevice].Name != "None")
			//	SetEmulatorImageAndSize();
			SetScreenLocationAndSize();
		}

		//TODO: create property and inject from EmulatorControl
		private MenuItem menuDevice = new MenuItem();
		private MenuItem menuOrientation = new MenuItem();
		private MenuItem menuScale = new MenuItem();

		private void SetEmulatorImageAndSize()
		{
			deviceImage.Image = Image.FromStream(GetImageFilestream(selectedDevice));
			if (orientationPortrait)
				deviceImage.Size = new Size((int)(deviceImage.Image.Size.Width * scale),
					(int)(deviceImage.Image.Size.Height * scale));
			else
				deviceImage.Size = new Size((int)(deviceImage.Image.Size.Height * scale),
					(int)(deviceImage.Image.Size.Width * scale));
			MinWidth = deviceImage.Size.Width;
			MinHeight = deviceImage.Size.Height;
		}

		private Stream GetImageFilestream(int deviceIndex)
		{
			var imageFilename = "Images.Emulators." + devices[deviceIndex].ImageFile + ".png";
			return EmbeddedResourcesLoader.GetEmbeddedResourceStream(imageFilename);
		}

		private void SetScreenLocationAndSize()
		{
			if (orientationPortrait)
				SetPortraitScreenLocationAndSize();
			else
				SetLandscapeScreenLocationAndSize();
		}

		private void SetPortraitScreenLocationAndSize()
		{
			Screen.Location = new Point((int)(devices[selectedDevice].ScreenPoint.X * scale),
				(int)(devices[selectedDevice].ScreenPoint.Y * scale));
			Screen.Size = new Size((int)(devices[selectedDevice].ScreenSize.Width * scale),
				(int)(devices[selectedDevice].ScreenSize.Height * scale));
		}

		private void SetLandscapeScreenLocationAndSize()
		{
			deviceImage.Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
			Screen.Location = new Point((int)(devices[selectedDevice].ScreenPoint.Y * scale),
				(int)(devices[selectedDevice].ScreenPoint.X * scale));
			Screen.Size = new Size((int)(devices[selectedDevice].ScreenSize.Height * scale),
				(int)(devices[selectedDevice].ScreenSize.Width * scale));
		}

		private void SelectScale(int scaleIndex)
		{
			string scalePercentage = menuScaleScales[scaleIndex].Text;
			scalePercentage = scalePercentage.Substring(0, scalePercentage.Length - 1);
			scale = int.Parse(scalePercentage) * 0.01f;
			for (int i = 0; i < NumberOfScales; i++)
				menuScaleScales[i].Checked = false;
			menuScaleScales[scaleIndex].Checked = true;
		}

		private MenuItem[] menuScaleScales = new MenuItem[0];

		private void SelectOrientation(int orientationIndex)
		{
			orientationPortrait = orientationIndex == 0;
			menuOrientationPortrait.Checked = orientationPortrait;
			menuOrientationLandscape.Checked = !orientationPortrait;
		}

		private Device[] devices = new Device[]
		{
			new Device()
			{
				Type = "Default",
				Name = "None",
				ScreenSize = new Size(1366, 768),
				CanScale = true,
				DefaultScaleIndex = 2
			}
		};
		private int selectedDevice = 0;
		private MenuItem menuOrientationPortrait = new MenuItem();
		private MenuItem menuOrientationLandscape = new MenuItem();

		private bool orientationPortrait = true;
		private float scale = 1.0f;

		private const int NumberOfScales = 3;

		/*

		private void SelectDevice(int deviceIndex)
		{
			selectedDevice = deviceIndex;
			for (int i = 0; i < devices.Length; i++)
				menuDeviceDevices[i].Checked = false;
			menuDeviceDevices[deviceIndex].Checked = true;
			menuOrientation.Visible = devices[deviceIndex].CanRotate;
			menuScale.Visible = devices[deviceIndex].CanScale;
			Screen.Dock = selectedDevice == 0 ? DockStyle.Fill : DockStyle.None;
		}

		 */

		public string ShortName
		{
			get { return "Viewport"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/Viewport.png"; }
		}

		public bool RequiresLargePane
		{
			get { return true; }
		}
	}
}