using System;
using DeltaEngine.Editor.Core;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public class AndroidAppInfoTests
	{
		[Test]
		public void CheckValuesOfSimpleAppInfo()
		{
			var appInfo = new AndroidAppInfo("MockApp.zip", Guid.Empty);
			Assert.AreEqual("MockApp.zip", appInfo.FilePath);
			Assert.AreEqual("MockApp", appInfo.Name);
			Assert.AreEqual(PlatformName.Android, appInfo.Platform);
			Assert.IsFalse(appInfo.IsSolutionPathAvailable);
		}

		[Test]
		public void CheckRebuildableApp()
		{
			var appInfo = new AndroidAppInfo("MockApp.zip", Guid.Empty) { SolutionFilePath = "App.sln" };
			Assert.IsTrue(appInfo.IsSolutionPathAvailable);
		}

		[Test]
		public void LaunchAppWithoutDeviceThrowsExcetpion()
		{
			AppInfo app = "FakeApp".AsMockAppInfo(PlatformName.Android);
			Assert.Throws<AppInfo.NoDeviceSpecified>(() => app.LaunchApp(null));
		}

		[Test, Category("Slow")]
		public void LaunchApp()
		{
			AppInfo app = "LogoApp".TryGetAlreadyBuiltApp(PlatformName.Android);
			if (app == null)
				return;
			Device[] availableDevices = app.AvailableDevices;
			if (availableDevices.Length > 0)
				app.LaunchApp(availableDevices[0]);
		}
	}
}