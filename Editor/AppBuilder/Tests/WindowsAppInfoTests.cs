using System;
using DeltaEngine.Editor.Core;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public class WindowsAppInfoTests
	{
		[Test]
		public void CheckValuesOfSimpleAppInfo()
		{
			var appInfo = new WindowsAppInfo("MockApp.zip", Guid.Empty);
			Assert.AreEqual("MockApp.zip", appInfo.FilePath);
			Assert.AreEqual("MockApp", appInfo.Name);
			Assert.AreEqual(PlatformName.Windows, appInfo.Platform);
			Assert.IsNotEmpty(appInfo.AvailableDevices);
			Assert.IsTrue(appInfo.IsDeviceAvailable);
			Assert.IsFalse(appInfo.IsSolutionPathAvailable);
		}

		[Test]
		public void CheckRebuildableApp()
		{
			var appInfo = new WindowsAppInfo("MockApp.zip", Guid.Empty) { SolutionFilePath = "App.sln"};
			Assert.IsTrue(appInfo.IsSolutionPathAvailable);
		}

		[Test]
		public void LaunchAppWithoutDeviceThrowsExcetpion()
		{
			AppInfo app = "FakeApp".AsMockAppInfo(PlatformName.Windows);
			Assert.Throws<AppInfo.NoDeviceSpecified>(() => app.LaunchApp(null));
		}

		[Test, Category("Slow")]
		public void LaunchApp()
		{
			var device = new WindowsDevice();
			AppInfo app = "LogoApp".TryGetAlreadyBuiltApp(PlatformName.Windows);
			if (app != null)
				app.LaunchApp(device);
		}
	}
}