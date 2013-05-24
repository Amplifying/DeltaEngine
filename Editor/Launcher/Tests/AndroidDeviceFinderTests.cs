using System;
using NUnit.Framework;

namespace DeltaEngine.Editor.Launcher.Tests
{
	internal class AndroidDeviceFinderTests
	{
		[Test]
		public void GetAvailableDevices()
		{
			var deviceFinder = new AndroidDeviceFinder();
			AndroidDevice[] availableDevices = deviceFinder.GetAvailableDevices();
			Console.WriteLine(availableDevices.Length + " devices available:");
			foreach (AndroidDevice device in availableDevices)
				Console.WriteLine("\t" + device);
			Assert.IsNotEmpty(availableDevices);
		}
	}
}