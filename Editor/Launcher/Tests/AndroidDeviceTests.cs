using System;
using System.IO;
using System.Linq;
using DeltaEngine.Editor.Builder;
using NUnit.Framework;

namespace DeltaEngine.Editor.Launcher.Tests
{
	public class AndroidDeviceTests
	{
		[Test]
		public void CheckforExistingEmulator()
		{
			AndroidDevice device = GetEmulatorDevice();
			if (device == null)
				// Currently no Android emulator is running
				return;

			Assert.IsTrue(device.IsEmulator);
		}

		[Test, Category("Slow")]
		public void InstallLogoAppOnEmulator()
		{
			AndroidDevice device = GetEmulatorDevice();
			var appPackage = GetAppPackage("LogoApp");
			device.Install(appPackage);
		}

		private static AppPackage GetAppPackage(string appName)
		{
			string packageFileName = appName + ".apk";
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

		private static AndroidDevice GetEmulatorDevice()
		{
			var deviceFinder = new AndroidDeviceFinder();
			return deviceFinder.GetAvailableDevices().FirstOrDefault(device => device.IsEmulator);
		}

		[Test, Category("Slow")]
		public void LaunchLogoAppOnEmulator()
		{
			var appPackage = GetAppPackage("LogoApp");
			AndroidDevice device = GetEmulatorDevice();
			device.Launch(appPackage);
		}

		[Test, Category("Slow")]
		public void InstallAndLaunchLogoAppOnEmulator()
		{
			var appPackage = GetAppPackage("LogoApp");
			AndroidDevice device = GetEmulatorDevice();
			device.Install(appPackage);
			device.Launch(appPackage);
		}
	}
}