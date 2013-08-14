using System.Collections.Generic;
using DeltaEngine.Editor.Core;
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
			Assert.AreEqual(0, appListViewModel.NumberOfBuiltApps);
			appListViewModel.AddApp("TestApp".AsMockAppInfo(PlatformName.Windows));
			Assert.AreEqual(1, appListViewModel.NumberOfBuiltApps);
		}

		[Test]
		public void SaveOrLoadWithoutAppStorageDirectoryShouldFail()
		{
			appListViewModel.AppStorageDirectory = "";
			Assert.Throws<BuiltAppsListViewModel.NoAppStorageDirectorySpecified>(appListViewModel.Load);
			Assert.Throws<BuiltAppsListViewModel.NoAppStorageDirectorySpecified>(
				() => AddMockAppInfoAndSaveWithDummyData(appListViewModel));
		}

		private void AddMockAppInfoAndSaveWithDummyData(BuiltAppsListViewModel viewModel)
		{
			string folder = appListViewModel.AppStorageDirectory;
			appListViewModel.AddApp("TestApp".AsMockAppInfo(PlatformName.Windows, folder), new byte[] { 1 });
		}

		[Test, Category("Slow")]
		public void SaveAndLoadBuiltApps()
		{
			appListViewModel.AppStorageDirectory = "SaveAndLoadBuiltApps";
			AddMockAppInfoAndSaveWithDummyData(appListViewModel);
			var loadedViewModel = new BuiltAppsListViewModel();
			loadedViewModel.AppStorageDirectory = appListViewModel.AppStorageDirectory;
			loadedViewModel.Load();
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