﻿using System;
using System.Windows.Input;
using ApprovalTests.Reporters;
using ApprovalTests.Wpf;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Messages;
using NUnit.Framework;
using WpfWindow = System.Windows.Window;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	[UseReporter(typeof(KDiffReporter))]
	public class AppBuilderViewTests
	{
		[Test, STAThread, Category("Slow")]
		public void VerifyViewWithMocking()
		{
			WpfApprovals.Verify(CreateTestWindow(CreateViewAndViewModelViaMockService()));
		}

		private static WpfWindow CreateTestWindow(AppBuilderView view)
		{
			return new WpfWindow
			{
				Content = view,
				Width = 1280,
				Height = 720,
				MinWidth = 800,
				MinHeight = 480
			};
		}

		private static AppBuilderView CreateViewAndViewModelViaMockService()
		{
			var service = new MockBuildService();
			var view = CreateViewWithInitialiedViewModel(service);
			service.ReceiveSomeTestMessages();
			return view;
		}

		private static AppBuilderView CreateViewWithInitialiedViewModel(Service service)
		{
			var view = new AppBuilderView();
			view.Init(service);
			return view;
		}
		
		[Test, STAThread, Category("Slow"), Category("WPF")]
		public void ShowViewWithMockServiceAndDummyApps()
		{
			AppBuilderView builderView = CreateViewAndViewModelViaMockService();
			AppBuilderViewModel viewModel = builderView.ViewModel;
			viewModel.AppListViewModel.AddApp(
				AppBuilderTestExtensions.GetMockAppInfo("My favorite app", PlatformName.Windows));
			viewModel.AppListViewModel.AddApp(AppBuilderTestExtensions.GetMockAppInfo(
				"My mobile app", PlatformName.WindowsPhone7));
			WpfWindow window = CreateTestWindow(builderView);
			window.ShowDialog();
		}

		[Test, STAThread, Category("Slow"), Category("WPF")]
		public void ShowViewWithMockServiceAndLoadedAppStorage()
		{
			AppBuilderView builderView = CreateViewAndViewModelViaMockService();
			WpfWindow window = CreateTestWindow(builderView);
			window.ShowDialog();
		}

		[Test, STAThread, Category("Slow"), Category("WPF")]
		public void ShowViewWithMockServiceToVisualizeSwitchingBetweenBothLists()
		{
			var service = new MockBuildService();
			AppBuilderView builderView = CreateViewWithInitialiedViewModel(service);
			AppBuilderViewModel viewModel = builderView.ViewModel;
			WpfWindow window = CreateTestWindow(builderView);
			window.MouseDoubleClick += (sender, e) => FireAppBuildMessagesOnMouseDoubleClick(e, viewModel);
			window.ShowDialog();
		}

		private static void FireAppBuildMessagesOnMouseDoubleClick(MouseButtonEventArgs e,
			AppBuilderViewModel viewModel)
		{
			if (e.LeftButton != MouseButtonState.Released)
				SelectProjectToBuild(viewModel, "LogoApp");
			else if (e.RightButton != MouseButtonState.Released)
				SelectProjectToBuild(viewModel, "GhostWars");
			viewModel.BuildCommand.Execute(null);
		}

		private static void SelectProjectToBuild(AppBuilderViewModel viewModel, string projectName)
		{
			viewModel.SelectedSolutionProject =
				viewModel.AvailableProjectsInSelectedSolution.Find(p => p.Title == projectName);
		}
	}
}