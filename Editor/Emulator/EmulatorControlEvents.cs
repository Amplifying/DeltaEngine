using System;
using System.Windows.Forms;

namespace DeltaEngine.Editor.Emulator
{
	public partial class EmulatorControl
	{
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
	}
}