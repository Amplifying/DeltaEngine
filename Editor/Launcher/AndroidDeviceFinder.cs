using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Editor.Builder;

namespace DeltaEngine.Editor.Launcher
{
	/// <summary>
	/// Finds Android devices which are connected to the current local machine.
	/// </summary>
	/// <see cref="http://www.petefreitag.com/item/763.cfm"/>
	/// <see cref="http://mobile.tutsplus.com/tutorials/connecting-physical-android-devices-to-your-development-machine/"/>
	/// <see cref="http://developer.android.com/tools/devices/emulator.html"/>
	/// <see cref="http://developer.android.com/tools/help/emulator.html"/>
	public class AndroidDeviceFinder
	{
		public AndroidDevice[] GetAvailableDevices()
		{
			var adbRunner = new AndroidDebugBridgeRunner();
			string[] namesOfDevices = adbRunner.GetInfosOfAvailableDevices();
			var deviceList = new AndroidDevice[namesOfDevices.Length];
			for (int i = 0; i < namesOfDevices.Length; i++)
				deviceList[i] = CreateAndroidDevice(namesOfDevices[i], adbRunner);

			return deviceList;
		}

		private static AndroidDevice CreateAndroidDevice(string deviceinfo,
			AndroidDebugBridgeRunner adbRunner)
		{
			string[] infoParts = deviceinfo.Split('\t');
			string adbDeviceId = infoParts[0];
			string deviceState = infoParts[1];
			return new AndroidDevice(adbRunner, adbDeviceId, deviceState);
		}
	}
}