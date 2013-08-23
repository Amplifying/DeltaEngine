using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DeltaEngine.Editor.Core;
using Microsoft.Win32;

namespace DeltaEngine.Editor.Emulator
{
	public partial class EmulatorControl
	{
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
			var xmlStream = EmbeddedResoucesLoader.GetEmbeddedResourceStream("Devices.Devices.xml");
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
	}
}