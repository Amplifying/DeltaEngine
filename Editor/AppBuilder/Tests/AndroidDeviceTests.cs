using System;
using System.Linq;
using DeltaEngine.Editor.Core;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	[Category("Slow")]
	public class AndroidDeviceTests
	{
		[TestFixtureSetUp]
		public void GetEmulatorDeviceAndSampleAppInfo()
		{
			firstDevice = GetFirstAvailableDevice();
			sampleApp = "LogoApp".TryGetAlreadyBuiltApp(PlatformName.Android);
		}

		private Device firstDevice;

		private static Device GetFirstAvailableDevice()
		{
			var deviceFinder = new AndroidDeviceFinder();
			return deviceFinder.GetAvailableDevices().FirstOrDefault();
		}

		private AppInfo sampleApp;

		[Test]
		public void CheckExistingDevice()
		{
			Console.WriteLine("Device is emulator: " + firstDevice.IsEmulator);
			Assert.IsTrue(firstDevice is AndroidDevice);
		}

		[Test, Category("Slow")]
		public void UninstallApp()
		{
			if (!firstDevice.IsAppInstalled(sampleApp))
				firstDevice.Install(sampleApp);
			Assert.IsTrue(firstDevice.IsAppInstalled(sampleApp));
			firstDevice.Uninstall(sampleApp);
			Assert.IsFalse(firstDevice.IsAppInstalled(sampleApp));
		}

		[Test, Category("Slow")]
		public void InstallApp()
		{
			if (firstDevice.IsAppInstalled(sampleApp))
				firstDevice.Uninstall(sampleApp);
			Assert.IsFalse(firstDevice.IsAppInstalled(sampleApp));
			firstDevice.Install(sampleApp);
			Assert.IsTrue(firstDevice.IsAppInstalled(sampleApp));
		}

		[Test, Category("Slow")]
		public void LaunchApp()
		{
			if (!firstDevice.IsAppInstalled(sampleApp))
				firstDevice.Install(sampleApp);
			firstDevice.Launch(sampleApp);
		}
	}
}