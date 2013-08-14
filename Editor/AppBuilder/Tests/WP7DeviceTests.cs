using System;
using System.IO;
using System.Linq;
using DeltaEngine.Editor.Core;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public class WP7DeviceTests
	{
		[TestFixtureSetUp]
		public void GetEmulatorDeviceAndSampleAppInfo()
		{
			emulatorDevice = GetEmulatorDevice();
			sampleApp = "LogoApp".TryGetAlreadyBuiltApp(PlatformName.WindowsPhone7);
		}

		private Device emulatorDevice;

		private static Device GetEmulatorDevice()
		{
			var deviceFinder = new WP7DeviceFinder();
			return deviceFinder.GetAvailableDevices().FirstOrDefault(device => device.IsEmulator);
		}

		private AppInfo sampleApp;
		
		[Test]
		public void CheckforExistingEmulator()
		{
			Assert.IsTrue(emulatorDevice.IsEmulator);
			Assert.IsTrue(emulatorDevice is WP7Device);
		}

		[Test, Category("Slow")]
		public void UninstallApp()
		{
			if (!emulatorDevice.IsAppInstalled(sampleApp))
				emulatorDevice.Install(sampleApp);
			Assert.IsTrue(emulatorDevice.IsAppInstalled(sampleApp));
			emulatorDevice.Uninstall(sampleApp);
			Assert.IsFalse(emulatorDevice.IsAppInstalled(sampleApp));
		}

		[Test, Category("Slow")]
		public void InstallApp()
		{
			if (emulatorDevice.IsAppInstalled(sampleApp))
				emulatorDevice.Uninstall(sampleApp);
			Assert.IsFalse(emulatorDevice.IsAppInstalled(sampleApp));
			emulatorDevice.Install(sampleApp);
			Assert.IsTrue(emulatorDevice.IsAppInstalled(sampleApp));
		}

		[Test, Category("Slow")]
		public void LaunchApp()
		{
			if (!emulatorDevice.IsAppInstalled(sampleApp))
				emulatorDevice.Install(sampleApp);
			emulatorDevice.Launch(sampleApp);
		}
	}
}