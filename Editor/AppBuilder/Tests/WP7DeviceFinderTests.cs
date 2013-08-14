using System;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public class WP7DeviceFinderTests
	{
		[Test]
		public void GetAvailableWP7Devices()
		{
			var deviceFinder = new WP7DeviceFinder();
			Device[] availableDevices = deviceFinder.GetAvailableDevices();
			foreach (Device device in availableDevices)
				AssertWP7Device(device);
			Assert.AreNotEqual(0, availableDevices.Length);
		}

		public void AssertWP7Device(Device device)
		{
			Console.WriteLine(device);
			Assert.IsTrue(device is WP7Device, device.GetType() + " - " + device.Name);
			Assert.IsNotEmpty(device.Name);
		}
	}
}