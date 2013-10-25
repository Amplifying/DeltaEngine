using System;
using DeltaEngine.Editor.AppBuilder.Windows;
using DeltaEngine.Editor.Messages;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests.Windows
{
	[Category("Slow")]
	public class WindowsDeviceTests
	{
		[TestFixtureSetUp]
		public void CreatePackageFileOfSample()
		{
			sampleApp = AppBuilderTestExtensions.TryGetAlreadyBuiltApp("LogoApp", PlatformName.Windows);
			device = new WindowsDevice();
		}

		private AppInfo sampleApp;
		private WindowsDevice device;

		[Test]
		public void UninstallOfAnInvalidAppShouldFail()
		{
			Assert.Throws<WindowsDevice.UninstallationFailedOnDevice>(() => device.Uninstall(null));
		}

		[Test]
		public void UninstallApp()
		{
			if (!device.IsAppInstalled(sampleApp))
				device.Install(sampleApp);
			Assert.IsTrue(device.IsAppInstalled(sampleApp));
			device.Uninstall(sampleApp);
			Assert.IsFalse(device.IsAppInstalled(sampleApp));
		}

		[Test]
		public void InstallApp()
		{
			if (device.IsAppInstalled(sampleApp))
				device.Uninstall(sampleApp);
			device.Install(sampleApp);
			Assert.IsTrue(device.IsAppInstalled(sampleApp));
		}

		[Test]
		public void LaunchIvalidApp()
		{
			Assert.Throws<WindowsDevice.AppNotInstalled>(() => device.Launch(null));
		}

		[Test]
		public void LaunchApp()
		{
			if (!device.IsAppInstalled(sampleApp))
				device.Install(sampleApp);
			device.Launch(sampleApp);
		}
	}
}