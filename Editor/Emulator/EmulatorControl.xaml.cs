using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Linq;
using DeltaEngine.Editor.Core;
using Microsoft.Win32;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

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
			EmulatorViewModel = new EmulatorViewModel();
			DataContext = EmulatorViewModel;
		}

		public void ProjectChanged() {}

		public EmulatorViewModel EmulatorViewModel { get; set; }

		/*
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

		private void SetupScreen()
		{
			Screen = new Panel();
			Screen.BackColor = Color.Purple;
			Screen.Dock = DockStyle.None;
		}

		private void SetupDeviceFrame()
		{
			deviceImage = new PictureBox();
			deviceImage.BackColor = Color.Transparent;
			deviceImage.BackgroundImageLayout = ImageLayout.Center;
			deviceImage.Location = new Point(0, 0);
			deviceImage.SizeMode = PictureBoxSizeMode.StretchImage;
		}

		private void SetupEmulator()
		{
			emulator = new Panel();
			emulator.Controls.Add(Screen);
			emulator.Controls.Add(deviceImage);
			emulator.Dock = DockStyle.Fill;
			emulator.Controls.Add(deviceImage);
		}

		private void SetupHost()
		{
			ViewportHost.Child = emulator;
			((ISupportInitialize)(deviceImage)).EndInit();
			emulator.ResumeLayout(false);
		}

		private MenuItem menuDevice;
		private MenuItem menuOrientation;
		private MenuItem menuScale;

		private MenuItem[] menuDeviceDevices;
		private MenuItem menuOrientationPortrait;
		private MenuItem menuOrientationLandscape;
		private MenuItem[] menuScaleScales;

		private void SetupMenu()
		{
			var menu = new ContextMenu();
			emulator.ContextMenu = menu;
			menuDevice = new MenuItem();
			menuDevice.Text = Properties.Resources.EmulatorControl_SetupMenu_Device;
			SetupDeviceMenu();
			menu.MenuItems.Add(menuDevice);
			menuOrientation = new MenuItem();
			menuOrientation.Text = Properties.Resources.EmulatorControl_SetupMenu_Orientation;
			SetupOrientationMenu();
			menu.MenuItems.Add(menuOrientation);
			menuScale = new MenuItem();
			menuScale.Text = Properties.Resources.EmulatorControl_SetupMenu_Scale;
			SetupScaleMenu();
			menu.MenuItems.Add(menuScale);
		}

		private void SetupDeviceMenu()
		{
			menuDeviceDevices = new MenuItem[devices.Length];
			string lastType = devices[0].Type;
			for (int i = 0; i < devices.Length; i++)
			{
				SetupDeviceMenuOption(i, lastType);
				lastType = devices[i].Type;
			}
		}

		private void SetupDeviceMenuOption(int optionIndex, string lastType)
		{
			if (devices[optionIndex].Type != lastType)
				menuDevice.MenuItems.Add("-");

			menuDeviceDevices[optionIndex] = new MenuItem();
			menuDeviceDevices[optionIndex].Tag = optionIndex;
			menuDeviceDevices[optionIndex].Text = devices[optionIndex].Name;
			menuDeviceDevices[optionIndex].Click += MenuDeviceClick;
			menuDevice.MenuItems.Add(menuDeviceDevices[optionIndex]);
		}

		private void SetupOrientationMenu()
		{
			menuOrientationPortrait = new MenuItem();
			menuOrientationPortrait.Checked = true;
			menuOrientationPortrait.Text =
				Properties.Resources.EmulatorControl_SetupOrientationMenu_Portrait;
			menuOrientationPortrait.Tag = 0;
			menuOrientationPortrait.Click += MenuOrientationClick;
			menuOrientation.MenuItems.Add(menuOrientationPortrait);
			menuOrientationLandscape = new MenuItem();
			menuOrientationLandscape.Checked = false;
			menuOrientationLandscape.Text =
				Properties.Resources.EmulatorControl_SetupOrientationMenu_Landscape;
			menuOrientationLandscape.Tag = 1;
			menuOrientationLandscape.Click += MenuOrientationClick;
			menuOrientation.MenuItems.Add(menuOrientationLandscape);
		}

		private void SetupScaleMenu()
		{
			menuScaleScales = new MenuItem[NumberOfScales];
			SetupScaleMenuOption(0, Properties.Resources.EmulatorControl_SetupScaleMenu__100);
			SetupScaleMenuOption(1, Properties.Resources.EmulatorControl_SetupScaleMenu__75);
			SetupScaleMenuOption(2, Properties.Resources.EmulatorControl_SetupScaleMenu__50);
			for (int i = 0; i < NumberOfScales; i++)
			{
				menuScaleScales[i].Click += MenuScaleClick;
				menuScale.MenuItems.Add(menuScaleScales[i]);
			}
		}

		private void SetupScaleMenuOption(int optionIndex, string text)
		{
			menuScaleScales[optionIndex] = new MenuItem
			{
				Checked = true,
				Tag = optionIndex,
				Text = text
			};
		}
		private void MenuDeviceClick(object sender, EventArgs e)
		{
			var selectedOption = (int)((MenuItem)sender).Tag;
			SelectDevice(selectedOption);
			SelectScale(devices[selectedOption].DefaultScaleIndex);
			Update();
		}

		private void MenuOrientationClick(object sender, EventArgs e)
		{
			var selectedOption = (int)((MenuItem)sender).Tag;
			SelectOrientation(selectedOption);
			Update();
		}

		private void MenuScaleClick(object sender, EventArgs e)
		{
			var selectedOption = (int)((MenuItem)sender).Tag;
			SelectScale(selectedOption);
			Update();
		}
		public void SaveStateToRegistry()
		{
			RegistryKey registryKey =
				Registry.CurrentUser.CreateSubKey(@"Software\DeltaEngine\Editor\Emulator");
			if (registryKey == null)
				return;
			registryKey.SetValue("Device", selectedDevice);
			registryKey.SetValue("Orientation", orientationPortrait ? 0 : 1);
			for (int i = 0; i < NumberOfScales; i++)
				if (menuScaleScales[i].Checked)
				{
					registryKey.SetValue("Scale", i);
					return;
				}
		}

		private void LoadStateFromRegistry()
		{
			RegistryKey registryKey =
				Registry.CurrentUser.OpenSubKey(@"Software\DeltaEngine\Editor\Emulator", false);
			if (registryKey != null)
			{
				SelectDevice((int)registryKey.GetValue("Device"));
				SelectOrientation((int)registryKey.GetValue("Orientation"));
				SelectScale((int)registryKey.GetValue("Scale"));
			}
			else
				LoadDefaults();
		}

		private void LoadDefaults()
		{
			SelectDevice(0);
			SelectScale(devices[selectedDevice].DefaultScaleIndex);
			SelectOrientation(0);
		}

		private void LoadDeviceInfoList()
		{
			var xmlStream =
				EmbeddedResourcesLoader.GetEmbeddedResourceStream("Images.Emulators.Devices.xml");
			XElement xmlFile = XElement.Load(xmlStream);
			IEnumerable<XElement> xmlDevices = xmlFile.Elements();
			var deviceCount = (from category in xmlFile.Descendants("Device") select category).Count();
			devices = new Device[deviceCount];
			int deviceIndex = 0;
			foreach (var xmlDevice in xmlDevices)
				LoadDeviceInfo(deviceIndex++, xmlDevice);
		}

		private void LoadDeviceInfo(int deviceIndex, XContainer xmlDevice)
		{
			devices[deviceIndex].Type = xmlDevice.ReadStringValue("Type");
			devices[deviceIndex].Name = xmlDevice.ReadStringValue("Name");
			devices[deviceIndex].ImageFile = xmlDevice.ReadStringValue("ImageFile");
			devices[deviceIndex].ScreenPoint = xmlDevice.ReadPointValue("ScreenPoint");
			devices[deviceIndex].ScreenSize = xmlDevice.ReadSizeValue("ScreenSize");
			devices[deviceIndex].CanRotate = xmlDevice.ReadBoolValue("CanRotate");
			devices[deviceIndex].CanScale = xmlDevice.ReadBoolValue("CanScale");
			devices[deviceIndex].DefaultScaleIndex = xmlDevice.ReadIntValue("DefaultScaleIndex");
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
		 */

		private void OnSelectedColorChanged(object sender,
			RoutedPropertyChangedEventArgs<System.Windows.Media.Color> e)
		{

		}

		public string ShortName
		{
			get { return "Emulator"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/Emulator.png"; }
		}

		public bool RequiresLargePane
		{
			get { return false; }
		}
	}
}