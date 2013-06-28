using System;
using System.Windows;
using ApprovalTests.Reporters;
using ApprovalTests.Wpf;
using DeltaEngine.Editor.Common;
using NUnit.Framework;

namespace DeltaEngine.Editor.Builder.Tests
{
	[UseReporter(typeof(KDiffReporter))]
	public class BuilderViewTests
	{
		[Test, STAThread, Category("Slow")]
		public void VerifyViewWithMocking()
		{
			WpfApprovals.Verify(CreateVerifiableWindowFromViewModel(CreateViewModelWithMockService()));
		}

		private static BuilderViewModel CreateViewModelWithMockService()
		{
			var mockService = new BuilderMockService();
			var viewModel = new BuilderViewModel(mockService);
			mockService.ReceiveSomeTestMessages();
			viewModel.AppListViewModel.BuiltApps.Add("My favorite app".AsBuiltApp(PlatformName.Windows));
			viewModel.AppListViewModel.BuiltApps.Add("My mobile app".AsBuiltApp(PlatformName.Android));

			return viewModel;
		}

		private static Window CreateVerifiableWindowFromViewModel(BuilderViewModel viewModel)
		{
			return new Window { Content = new BuilderView(viewModel), Width = 800, Height = 480 };
		}

		[Test, STAThread, Category("Slow")]
		public void ShowViewWithMockService()
		{
			var window = CreateVerifiableWindowFromViewModel(CreateViewModelWithMockService());
			window.ShowDialog();
		}

		[Test, STAThread, Category("Slow")]
		public void ShowViewWithRealService()
		{
			var viewModel = CreateViewModelWithRealConnection();
			var window = CreateVerifiableWindowFromViewModel(viewModel);
			viewModel.SelectedPlatform = viewModel.SupportedPlatforms[0];
			window.ShowDialog();
		}

		private static BuilderViewModel CreateViewModelWithRealConnection()
		{
			var editorModel = new EditorViewModel(null);
			return new BuilderViewModel(editorModel.Service);
		}
	}
}