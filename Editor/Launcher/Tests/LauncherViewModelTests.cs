using System;
using System.IO;
using NUnit.Framework;

namespace DeltaEngine.Editor.Launcher.Tests
{
	public class LauncherViewModelTests
	{
		[Test]
		public void CheckAvailableDevicesForNoSelectedPackage()
		{
			LauncherViewModel viewModel = GetLauncherViewModel();
			Assert.IsEmpty(viewModel.AvailableDevices);
			Assert.IsNull(viewModel.SelectedDevice);
		}

		[Test]
		public void CheckAvailableDevicesForWP7Package()
		{
			LauncherViewModel viewModel = GetLauncherViewModel("MyPackage.xap");
			Device[] availableDevices = viewModel.AvailableDevices;
			Assert.IsNotEmpty(availableDevices);
			Assert.IsNotNull(viewModel.SelectedDevice);
		}

		[Test]
		public void CheckAvailableDevicesForAndroidPackage()
		{
			LauncherViewModel viewModel = GetLauncherViewModel("MyPackage.apk");
			Device[] availableDevices = viewModel.AvailableDevices;
			Assert.IsNotEmpty(availableDevices);
			Assert.IsNotNull(viewModel.SelectedDevice);
		}

		[Test]
		public void CheckAvailableDevicesForUnknownPlatformPackage()
		{
			LauncherViewModel viewModel = GetLauncherViewModel("MyPackage.abc");
			Assert.IsEmpty(viewModel.AvailableDevices);
			Assert.IsNull(viewModel.SelectedDevice);
		}

		private static LauncherViewModel GetLauncherViewModel(string selectedPackageFilePath = null)
		{
			return new LauncherViewModel(new MockLauncherService())
			{
				SelectedPackageFilePath = selectedPackageFilePath,
			};
		}

		[Test, Category("Slow")]
		public void LaunchPackageOnWP7Emulator()
		{
			var viewModel = GetLauncherViewModelWithPreSelectedApp("LogoApp.xap");
			viewModel.SelectedDevice = GetEmulatorDevice(viewModel.AvailableDevices);
			Assert.IsTrue(viewModel.LaunchPressed.CanExecute(null));
			viewModel.LaunchPressed.Execute(null);
		}

		internal static LauncherViewModel GetLauncherViewModelWithPreSelectedApp(string appFileName)
		{
			return GetLauncherViewModel(Path.Combine(@"\\Win7-VM\BuildServiceServer\AppDatabase",
				appFileName));
		}

		private static Device GetEmulatorDevice(Device[] availableDevices)
		{
			foreach (Device device in availableDevices)
				if (device.IsEmulator)
					return device;

			throw new WP7DeviceTests.NoEmulatorDeviceAvailable();
		}

		[Test, Category("Slow")]
		public void LaunchPackageOnAndroidEmulator()
		{
			var viewModel = GetLauncherViewModelWithPreSelectedApp("LogoApp.apk");
			viewModel.SelectedDevice = GetEmulatorDevice(viewModel.AvailableDevices);
			Assert.IsTrue(viewModel.LaunchPressed.CanExecute(null));
			viewModel.LaunchPressed.Execute(null);
		}

		[Test, Category("Slow")]
		public void LaunchPackageOnConnectedWP7Device()
		{
			var viewModel = GetLauncherViewModelWithPreSelectedApp("LogoApp");
			viewModel.SelectedDevice = GetConnectedDevice(viewModel.AvailableDevices);
			Assert.IsTrue(viewModel.LaunchPressed.CanExecute(null));
			viewModel.LaunchPressed.Execute(null);
		}

		private static Device GetConnectedDevice(Device[] availableDevices)
		{
			foreach (Device device in availableDevices)
				if (!device.IsEmulator)
					return device;

			throw new NoConnectedDeviceAvailable();
		}

		public class NoConnectedDeviceAvailable : Exception {}
	}
}