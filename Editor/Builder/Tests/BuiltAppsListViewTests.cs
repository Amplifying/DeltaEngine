using System;
using System.Windows;
using DeltaEngine.Editor.Common;
using NUnit.Framework;

namespace DeltaEngine.Editor.Builder.Tests
{
	public class BuiltAppsListViewTests
	{
		[Test, STAThread, Category("Slow")]
		public void ShowViewWithOneBuiltApp()
		{
			var listViewModel = GetBuiltAppsListWithDummyEntry();
			var window = CreateVerifiableWindowFromViewModel(listViewModel);
			window.ShowDialog();
		}

		private static BuiltAppsListViewModel GetBuiltAppsListWithDummyEntry()
		{
			var listViewModel = new BuiltAppsListViewModel();
			listViewModel.BuiltApps.Add("Windows app".AsBuiltApp(PlatformName.Windows));

			return listViewModel;
		}

		private static Window CreateVerifiableWindowFromViewModel(
			BuiltAppsListViewModel listViewModel)
		{
			var appsListView = new BuiltAppsListView();
			appsListView.DataContext = listViewModel;

			return new Window
			{
				Content = appsListView,
				Width = 800,
				Height = 480
			};
		}

		[Test, STAThread, Category("Slow")]
		public void ShowView()
		{
			var listViewModel = GetBuiltAppsListWithDummyEntry();
			listViewModel.BuiltApps.Add("WP7 app".AsBuiltApp(PlatformName.WindowsPhone7));
			listViewModel.BuiltApps.Add("Android app".AsBuiltApp(PlatformName.Android));
			var window = CreateVerifiableWindowFromViewModel(listViewModel);
			window.ShowDialog();
		}
	}
}
