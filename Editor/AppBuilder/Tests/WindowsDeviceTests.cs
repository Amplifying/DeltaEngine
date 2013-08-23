using System;
using System.IO;
using DeltaEngine.Editor.Core;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	[Category("Slow"), Ignore]
	public class WindowsDeviceTests
	{
		[TestFixtureSetUp]
		public void CreatePackageFileOfSample()
		{
			string packageFilePathOfSample = Path.Combine(PathExtensions.GetDeltaEngineDirectory(), "..",
				"DeltaEngineServices", "GeneratedCode", "LogoAppSetup.exe");
			sampleApp = AppInfoExtensions.CreateAppInfo(packageFilePathOfSample, PlatformName.Windows,
				new Guid("4d33a50e-3aa2-4e7e-bc0c-4ef7b3d5e985"));
			device = new WindowsDevice();
		}

		private AppInfo sampleApp;
		private WindowsDevice device;

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
			Assert.IsFalse(device.IsAppInstalled(sampleApp));
			device.Install(sampleApp);
			Assert.IsTrue(device.IsAppInstalled(sampleApp));
		}

		[Test]
		public void LaunchApp()
		{
			Console.WriteLine(Environment.CurrentDirectory);
			if (!device.IsAppInstalled(sampleApp))
				device.Install(sampleApp);
			device.Launch(sampleApp);
		}
	}
}