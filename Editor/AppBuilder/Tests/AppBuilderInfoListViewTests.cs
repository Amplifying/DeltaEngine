using System;
using DeltaEngine.Editor.Core;
using NUnit.Framework;
using WpfWindow = System.Windows.Window;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public class AppBuilderInfoListViewTests
	{
		[Test, STAThread, Category("Slow"), Category("WPF")]
		public void ShowViewWithMockService()
		{
			var infoListView = new AppBuilderInfoListView();
			infoListView.MessagesViewModel = CreateViewModelWithDummyMessages();
			infoListView.AppListViewModel = CreateAppsListViewModelWithDummyEntries();
			infoListView.FocusBuiltAppsList();
			var window = CreateVerifiableWindowFromViewModel(infoListView);
			window.ShowDialog();
		}

		private static AppBuildMessagesListViewModel CreateViewModelWithDummyMessages()
		{
			var listViewModel = new AppBuildMessagesListViewModel();
			listViewModel.AddMessage("A simple build warning".AsBuildTestWarning());
			listViewModel.AddMessage("A simple build error".AsBuildTestError());
			listViewModel.AddMessage("A second simple build error".AsBuildTestError());
			return listViewModel;
		}

		private static BuiltAppsListViewModel CreateAppsListViewModelWithDummyEntries()
		{
			var appListViewModel = new BuiltAppsListViewModel();
			appListViewModel.AddApp("A Windows app".AsMockAppInfo(PlatformName.Windows));
			appListViewModel.AddApp("A Windows Phone 7 app".AsMockAppInfo(PlatformName.WindowsPhone7));
			return appListViewModel;
		}

		private static WpfWindow CreateVerifiableWindowFromViewModel(AppBuilderInfoListView view)
		{
			return new WpfWindow { Content = view, Width = 800, Height = 480 };
		}
	}
}
