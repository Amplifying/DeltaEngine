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
			var deviceFinder = new AndroidDeviceFinder();
			Device[] availableDevices = deviceFinder.GetAvailableDevices();
			Console.WriteLine(availableDevices.Length + " devices available:");
			foreach (Device device in availableDevices)
				AssertAndroidDevice(device);
		}

		public void AssertAndroidDevice(Device device)
		{
			Console.WriteLine("\t" + device);
			Assert.IsTrue(device is AndroidDevice, device.GetType() + " - " + device.Name);
			Assert.IsNotEmpty(device.Name);
		}
	}
}