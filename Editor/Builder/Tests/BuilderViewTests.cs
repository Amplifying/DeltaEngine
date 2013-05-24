using System;
using System.Windows;
using ApprovalTests.Reporters;
using ApprovalTests.Wpf;
using NUnit.Framework;

namespace DeltaEngine.Editor.Builder.Tests
{
	[UseReporter(typeof(KDiffReporter))]
	public class BuilderViewTests
	{
		[Test, STAThread, Category("Slow")]
		public void VerifyViewWithMocking()
		{
			WpfApprovals.Verify(CreateVerifiableWindowFromViewModel(CreateMockViewModel()));
		}

		private static Window CreateVerifiableWindowFromViewModel(BuilderViewModel viewModel)
		{
			return new Window { Content = new BuilderView(viewModel), Width = 640, Height = 480 };
		}

		private static BuilderViewModel CreateMockViewModel()
		{
			return new MockBuilderViewModel();
		}

		[Test, STAThread, Ignore]
		public void ShowViewWithEmptyViewModel()
		{
			var window = CreateVerifiableWindowFromViewModel(CreateEmptyViewModel());
			window.ShowDialog();
		}

		private static BuilderViewModel CreateEmptyViewModel()
		{
			return new BuilderViewModel(new MockBuilderService());
		}

		[Test, STAThread, Ignore]
		public void ShowViewWithFullyMocking()
		{
			var window = CreateVerifiableWindowFromViewModel(CreateMockViewModel());
			window.ShowDialog();
		}

		[Test, STAThread, Ignore]
		public void ShowViewWithMockedModelButRealNetworking()
		{
			var viewModel = CreateViewModelWithRealConnection();
			var window = CreateVerifiableWindowFromViewModel(viewModel);
			viewModel.SelectedPlatform = viewModel.SupportedPlatforms[0];
			viewModel.UserProjectPath = "LogoApp.sln";
			window.ShowDialog();
		}

		private static BuilderViewModel CreateViewModelWithRealConnection()
		{
			return new BuilderViewModel(new BuildServiceConnectionViaLAN());
		}
	}
}