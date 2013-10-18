using System.Collections.Generic;
using System.IO;
using System.Linq;
using DeltaEngine.Editor.Messages;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public class BuiltAppsListViewModelTests
	{
		[SetUp]
		public void CreateAppsListViewModel()
		{
			appListViewModel = new BuiltAppsListViewModel();
		}

		private BuiltAppsListViewModel appListViewModel;

		[Test]
		public void AddInvalidBuiltAppShouldThrowException()
		{
			Assert.Throws<BuiltAppsListViewModel.NoAppInfoSpecified>(() => appListViewModel.AddApp(null));
		}

		[Test]
		public void AddValidBuiltApp()
		{
			int oldNumberOfBuiltApps = appListViewModel.NumberOfBuiltApps;
			appListViewModel.AddApp(AppBuilderTestExtensions.GetMockAppInfo("TestApp",
				PlatformName.Windows));
			Assert.AreEqual(oldNumberOfBuiltApps + 1, appListViewModel.NumberOfBuiltApps);
		}

		[Test]
		public void IfApkFileExistOneAndroidPackageShouldBeInTheList()
		{
			if (!Directory.Exists(appListViewModel.AppStorageDirectory))
				return;
			Assert.True(appListViewModel.BuiltApps.Any(app => app.Platform == PlatformName.Android));
			Assert.True(appListViewModel.BuiltApps.Any(app => app.Platform == PlatformName.Web));
		}

		private void AddMockAppInfoAndSaveWithDummyData(BuiltAppsListViewModel viewModel)
		{
			string folder = appListViewModel.AppStorageDirectory;
			appListViewModel.AddApp(
				AppBuilderTestExtensions.GetMockAppInfo("TestApp", PlatformName.Windows, folder),
				new byte[] { 1 });
		}

		[Test, Category("Slow")]
		public void SaveAndLoadBuiltApps()
		{
			AddMockAppInfoAndSaveWithDummyData(appListViewModel);
			var loadedViewModel = new BuiltAppsListViewModel();
			AssertBuiltApps(appListViewModel.BuiltApps, loadedViewModel.BuiltApps);
		}

		private static void AssertBuiltApps(IList<AppInfo> savedApps, IList<AppInfo> loadedApps)
		{
			Assert.AreEqual(savedApps.Count, loadedApps.Count);
			for (int i = 0; i < loadedApps.Count; i++)
				AssertBuiltApp(savedApps[i], loadedApps[i]);
		}

		private static void AssertBuiltApp(AppInfo savedApp, AppInfo loadedApp)
		{
			Assert.AreEqual(savedApp.Name, loadedApp.Name);
			Assert.AreEqual(savedApp.Platform, loadedApp.Platform);
			Assert.AreEqual(savedApp.AppGuid, loadedApp.AppGuid);
			Assert.AreEqual(savedApp.FilePath, loadedApp.FilePath);
		}
	}
}