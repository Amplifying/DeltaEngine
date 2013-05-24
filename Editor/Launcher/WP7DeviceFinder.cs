using System;
using System.Collections.Generic;
using Microsoft.SmartDevice.Connectivity;
using MsDevice = Microsoft.SmartDevice.Connectivity.Device;

namespace DeltaEngine.Editor.Launcher
{
	public class WP7DeviceFinder
	{
		public WP7DeviceFinder()
		{
			devicesStorage = new DatastoreManager(1033);
		}

		private readonly DatastoreManager devicesStorage;

		public WP7Device[] GetAvailableDevices()
		{
			var devices = new List<WP7Device>();
			Platform nativePlatform = GetNativePlatform();
			foreach (MsDevice platformDevice in nativePlatform.GetDevices())
				devices.Add(new WP7Device(platformDevice));

			return devices.ToArray();
		}

		private Platform GetNativePlatform()
		{
			foreach (Platform platform in devicesStorage.GetPlatforms())
				if (platform.Name == "Windows Phone 7")
					return platform;

			throw new PlatformNotFoundException();
		}

		public class PlatformNotFound : Exception
		{
			public PlatformNotFound(string nativePlatformName) : base(nativePlatformName) {}
		}
	}
}