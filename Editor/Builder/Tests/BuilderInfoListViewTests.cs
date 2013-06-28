using System;
using System.Windows;
using DeltaEngine.Editor.Common;
using NUnit.Framework;

namespace DeltaEngine.Editor.Builder.Tests
{
	public class BuilderInfoListViewTests
	{
		[Test, STAThread, Category("Slow")]
		public void ShowViewWithMockService()
		{
			var infoListView = new BuilderInfoListView();
			infoListView.MessagesViewModel = CreateViewModelWithDummyMessages();
			infoListView.AppListViewModel = CreateAppsListViewModelWithDummyEntries();
			infoListView.FocusBuiltAppsList();
			var window = CreateVerifiableWindowFromViewModel(infoListView);
			window.ShowDialog();
		}

		private static BuildMessagesListViewModel CreateViewModelWithDummyMessages()
		{
			var listViewModel = new BuildMessagesListViewModel();
			listViewModel.AddMessage("A simple build warning".AsBuildTestWarning());
			listViewModel.AddMessage("A simple build error".AsBuildTestError());
			listViewModel.AddMessage("A second simple build error".AsBuildTestError());
			return listViewModel;
		}

		private static BuiltAppsListViewModel CreateAppsListViewModelWithDummyEntries()
		{
			var appListViewModel = new BuiltAppsListViewModel();
			appListViewModel.BuiltApps.Add("A Windows app".AsBuiltApp(PlatformName.Windows));
			appListViewModel.BuiltApps.Add("An Android app".AsBuiltApp(PlatformName.Android));

			return appListViewModel;
		}

		private static Window CreateVerifiableWindowFromViewModel(BuilderInfoListView view)
		{
			return new Window { Content = view, Width = 800, Height = 480 };
		}

	}
}
