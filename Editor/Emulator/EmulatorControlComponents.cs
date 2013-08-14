using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DeltaEngine.Editor.Emulator
{
	public partial class EmulatorControl
	{
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
	}
}