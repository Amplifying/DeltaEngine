using System;
using DeltaEngine.Editor.Messages;
using DeltaEngine.Logging;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public class AndroidAppInfoTests
	{
		[Test]
		public void CheckValuesOfSimpleAppInfo()
		{
			var appInfo = new AndroidAppInfo("MockApp.zip", Guid.Empty, DateTime.Now);
			Assert.AreEqual("MockApp.zip", appInfo.FilePath);
			Assert.AreEqual("MockApp", appInfo.Name);
			Assert.AreEqual(PlatformName.Android, appInfo.Platform);
			Assert.IsFalse(appInfo.IsSolutionPathAvailable);
		}

		[Test]
		public void CheckRebuildableApp()
		{
			var appInfo = new AndroidAppInfo("MockApp.zip", Guid.Empty, DateTime.Now)
			{
				SolutionFilePath = "App.sln"
			};
			Assert.IsTrue(appInfo.IsSolutionPathAvailable);
		}

		[Test]
		public void LaunchAppWithoutDeviceThrowsExcetpion()
		{
			AppInfo app = AppBuilderTestingExtensions.GetMockAppInfo("FakeApp", PlatformName.Android);
			Assert.Throws<AppInfo.NoDeviceSpecified>(() => app.LaunchApp(null));
		}

		[Test, Category("Slow")]
		public void LaunchApp()
		{
			new ConsoleLogger();
			const string AppName = "LogoApp";//"GhostWars";
			AppInfo app = AppBuilderTestingExtensions.TryGetAlreadyBuiltApp(AppName, PlatformName.Android);
			if (app == null)
				throw new Exception("No app found with name: " + AppName);
			Device[] availableDevices = AndroidDeviceFinder.GetAvailableDevices();
			if (availableDevices.Length > 0)
				app.LaunchApp(availableDevices[0]);
			else
				throw new Exception("No devices available to deploy to");
		}
	}
}