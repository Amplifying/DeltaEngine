using System;
using DeltaEngine.Editor.Core;
using NUnit.Framework;
using WpfWindow = System.Windows.Window;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public class BuiltAppsListViewTests
	{
		[Test, STAThread, Category("Slow"), Category("WPF")]
		public void ShowViewWithOneBuiltApp()
		{
			var listViewModel = GetBuiltAppsListWithDummyEntry();
			var window = CreateVerifiableWindowFromViewModel(listViewModel);
			window.ShowDialog();
		}

		private static BuiltAppsListViewModel GetBuiltAppsListWithDummyEntry()
		{
			var listViewModel = new BuiltAppsListViewModel();
			listViewModel.AddApp("Windows app".AsMockAppInfo(PlatformName.Windows));
			return listViewModel;
		}

		private static WpfWindow CreateVerifiableWindowFromViewModel(
			BuiltAppsListViewModel listViewModel)
		{
			var appsListView = new BuiltAppsListView();
			appsListView.ViewModel = listViewModel;
			return new WpfWindow { Content = appsListView, Width = 800, Height = 480 };
		}

		[Test, STAThread, Category("Slow"), Category("WPF")]
		public void ShowViewWithSeveralAppEntries()
		{
			var listViewModel = GetBuiltAppsListWithDummyEntry();
			listViewModel.AddApp("Windows app".AsMockAppInfo(PlatformName.Windows));
			listViewModel.AddApp("WP7 app".AsMockAppInfo(PlatformName.WindowsPhone7));
			var window = CreateVerifiableWindowFromViewModel(listViewModel);
			window.ShowDialog();
		}

		[Test, STAThread, Category("Slow"), Category("WPF")]
		public void ShowViewWithLogoAppForWindows()
		{
			var listViewModel = new BuiltAppsListViewModel();
			AppInfo app = "LogoApp".TryGetAlreadyBuiltApp(PlatformName.Windows);
			if (app != null)
				listViewModel.AddApp(app);
			var window = CreateVerifiableWindowFromViewModel(listViewModel);
			window.ShowDialog();
		}
	}
}