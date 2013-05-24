using System;
using System.IO;
using DeltaEngine.Editor.Builder;
using NUnit.Framework;

namespace DeltaEngine.Editor.Launcher.Tests
{
	public class WP7DeviceTests
	{
		[Test]
		public void CheckforExistingEmulator()
		{
			WP7Device device = GetEmulatorDevice();
			Assert.IsTrue(device.IsEmulator);
		}

		private static WP7Device GetEmulatorDevice()
		{
			var deviceFinder = new WP7DeviceFinder();
			foreach (WP7Device wp7Device in deviceFinder.GetAvailableDevices())
				if (wp7Device.IsEmulator)
					return wp7Device;

			throw new NoEmulatorDeviceAvailable();
		}

		public class NoEmulatorDeviceAvailable : Exception { }

		[Test, Category("Slow")]
		public void InstallAndLaunchLogoAppOnEmulator()
		{
			var appPackage = GetAppPackage("LogoApp");
			WP7Device device = GetEmulatorDevice();
			if (device.IsAppInstalled(appPackage))
				device.Uninstall(appPackage);
			device.Install(appPackage);
			device.Launch(appPackage);
		}

		private static AppPackage GetAppPackage(string appName)
		{
			string packageFileName = appName + ".xap";
			string packageFilePath = Path.Combine(@"\\Win7-VM\BuildServiceServer\AppDatabase",
				packageFileName);
			Assert.IsTrue(File.Exists(packageFilePath));
			return new AppPackage
			{
				AppName = appName,
				PackageFileName = packageFileName,
				PackageFileData = File.ReadAllBytes(packageFilePath),
				PackageGuid = LauncherViewModel.LogoAppAssemblyGuid,
			};
		}

	}
}