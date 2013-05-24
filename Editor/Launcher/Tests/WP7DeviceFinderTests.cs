using System;
using NUnit.Framework;
using MsDevice = Microsoft.SmartDevice.Connectivity.Device;

namespace DeltaEngine.Editor.Launcher.Tests
{
	public class WP7DeviceFinderTests
	{
		[Test]
		public void GetAvailableWP7Devices()
		{
			var deviceFinder = new WP7DeviceFinder();
			Device[] availableDevices = deviceFinder.GetAvailableDevices();
			foreach (Device device in availableDevices)
				Console.WriteLine(device);
			Assert.AreNotEqual(0, availableDevices.Length);
		}
	}
}