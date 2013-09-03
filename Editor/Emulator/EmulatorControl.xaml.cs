using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor.Emulator
{
	/// <summary>
	/// Change emulation of devices for the Engine Viewport
	/// </summary>
	public partial class EmulatorControl : EditorPluginView
	{
		public EmulatorControl()
		{
			InitializeComponent();
		}

		public void Init(Service service)
		{
			Devices = new Devices();
			DataContext = Devices;
		}

		public Devices Devices { get; set; }

		public void ApplyEmulator()
		{
			SetupScreen();
			SetupDeviceFrame();
			SetupEmulator();
			SetupHost();
			LoadDeviceInfoList();
			SetupMenu();
			LoadStateFromRegistry();
			Update();
		}

		public Panel Screen { get; private set; }
		private PictureBox deviceImage;
		private Panel emulator;

		private Device[] devices;
		private int selectedDevice;
		private bool orientationPortrait = true;
		private float scale = 1.0f;

		private const int NumberOfScales = 3;

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

		private void Update()
		{
			if (!menuOrientation.Visible)
				SelectOrientation(0);

			if (!menuScale.Visible)
				SelectScale(0);

			if (devices[selectedDevice].Name != "None")
				SetEmulatorImageAndSize();
			SetScreenLocationAndSize();
		}

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
			scale = Int32.Parse(scalePercentage) * 0.01f;
			for (int i = 0; i < NumberOfScales; i++)
				menuScaleScales[i].Checked = false;
			menuScaleScales[scaleIndex].Checked = true;
		}

		private void SelectOrientation(int orientationIndex)
		{
			orientationPortrait = orientationIndex == 0;
			menuOrientationPortrait.Checked = orientationPortrait;
			menuOrientationLandscape.Checked = !orientationPortrait;
		}

		public string ShortName
		{
			get { return "Viewport"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/Emulator.png"; }
		}

		public bool RequiresLargePane
		{
			get { return true; }
		}
	}
}