using System;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	[Category("Slow")]
	public class AndroidDeviceFinderTests
	{
		[Test]
		public void GetAvailableDevices()
		{
			Device[] availableDevices = AndroidDeviceFinder.GetAvailableDevices();
			foreach (Device device in availableDevices)
				AssertAndroidDevice(device);
		}

		public void AssertAndroidDevice(Device device)
		{
			Assert.IsTrue(device is AndroidDevice, device.GetType() + " - " + device.Name);
			Assert.IsNotEmpty(device.Name);
		}
	}
}