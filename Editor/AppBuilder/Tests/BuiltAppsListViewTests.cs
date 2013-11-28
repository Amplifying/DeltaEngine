using System;
using DeltaEngine.Editor.Messages;
using DeltaEngine.Mocks;
using NUnit.Framework;
using WpfWindow = System.Windows.Window;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	[Category("Slow"), Category("WPF")]
	public class BuiltAppsListViewTests
	{
		[Test, STAThread]
		public void ShowViewWithOneBuiltApp()
		{
			var listViewModel = GetBuiltAppsListWithDummyEntry();
			var window = CreateVerifiableWindowFromViewModel(listViewModel);
			window.ShowDialog();
		}

		private static BuiltAppsListViewModel GetBuiltAppsListWithDummyEntry()
		{
			var listViewModel = new BuiltAppsListViewModel(new MockSettings());
			listViewModel.AddApp(AppBuilderTestExtensions.GetMockAppInfo("Windows app",
				PlatformName.Windows));
			return listViewModel;
		}

		private static WpfWindow CreateVerifiableWindowFromViewModel(
			BuiltAppsListViewModel listViewModel)
		{
			var appsListView = new BuiltAppsListView();
			appsListView.ViewModel = listViewModel;
			return new WpfWindow { Content = appsListView, Width = 800, Height = 480 };
		}

		[Test, STAThread]
		public void ShowViewWithSeveralAppEntries()
		{
			var listViewModel = GetBuiltAppsListWithDummyEntry();
			listViewModel.AddApp(AppBuilderTestExtensions.GetMockAppInfo("Windows app",
				PlatformName.Windows));
			listViewModel.AddApp(AppBuilderTestExtensions.GetMockAppInfo("Android app",
				PlatformName.Android));
			listViewModel.AddApp(AppBuilderTestExtensions.GetMockAppInfo("Web app",
				PlatformName.Web));
			listViewModel.AddApp(AppBuilderTestExtensions.GetMockAppInfo("WP7 app",
				PlatformName.WindowsPhone7));
			var window = CreateVerifiableWindowFromViewModel(listViewModel);
			window.ShowDialog();
		}

		[Test, STAThread]
		public void ShowViewWithLogoAppForWindows()
		{
			var listViewModel = new BuiltAppsListViewModel(new MockSettings());
			AppInfo app = AppBuilderTestExtensions.TryGetAlreadyBuiltApp("LogoApp", PlatformName.Windows);
			listViewModel.AddApp(app);
			var window = CreateVerifiableWindowFromViewModel(listViewModel);
			window.ShowDialog();
		}
	}
}