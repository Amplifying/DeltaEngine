using System.Collections.Generic;

namespace DeltaEngine.Editor.Emulator
{
	public class Devices
	{
		public List<string> AvailableDevices { get; set; }
		public string SelectedDevice { get; set; }
		public List<string> AvailableScales { get; set; }
		public string SelectedScale { get; set; }
	}
}