using System;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Messages;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public class WP7AppInfoTests
	{
		[Test]
		public void CheckValuesOfSimpleAppInfo()
		{
			var appInfo = new WP7AppInfo("MockApp.zip", Guid.Empty, DateTime.Now);
			Assert.AreEqual("MockApp.zip", appInfo.FilePath);
			Assert.AreEqual("MockApp", appInfo.Name);
			Assert.AreEqual(PlatformName.WindowsPhone7, appInfo.Platform);
			Assert.IsFalse(appInfo.IsSolutionPathAvailable);
		}

		[Test]
		public void CheckRebuildableApp()
		{
			var appInfo = new WP7AppInfo("MockApp.zip", Guid.Empty, DateTime.Now)
			{
				SolutionFilePath = "App.sln"
			};
			Assert.IsTrue(appInfo.IsSolutionPathAvailable);
		}

		[Test]
		public void LaunchAppWithoutDeviceThrowsExcetpion()
		{
			AppInfo app = AppBuilderTestingExtensions.GetMockAppInfo("FakeApp",
				PlatformName.WindowsPhone7);
			Assert.Throws<AppInfo.NoDeviceSpecified>(() => app.LaunchApp(null));
		}

		/*TODO: this is stupid, exactly the same code as in AndroidAppInfoTests
		[Test, Category("Slow")]
		public void LaunchApp()
		{
			AppInfo app = AppBuilderTestingExtensions.TryGetAlreadyBuiltApp("LogoApp",
				PlatformName.WindowsPhone7);
			if (app == null)
				return;
			Device[] availableDevices = app.AvailableDevices;
			if (availableDevices.Length > 0)
				app.LaunchApp(availableDevices[0]);
		}
		 */
	}
}