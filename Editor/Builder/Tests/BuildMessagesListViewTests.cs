using System;
using System.Windows;
using ApprovalTests.Reporters;
using ApprovalTests.Wpf;
using NUnit.Framework;

namespace DeltaEngine.Editor.Builder.Tests
{
	[UseReporter(typeof(KDiffReporter))]
	public class BuildMessagesListViewTests
	{
		[Test, STAThread, Category("Slow")]
		public void VerifyViewWithMocking()
		{
			var listViewModel = CreateViewModelWithDummyMessages();
			WpfApprovals.Verify(CreateVerifiableWindowFromViewModel(listViewModel));
		}

		private static Window CreateVerifiableWindowFromViewModel(
			BuildMessagesListViewModel listViewModel)
		{
			return new Window
			{
				Content = new BuildMessagesListView(listViewModel),
				Width = 800,
				Height = 480
			};
		}

		private static BuildMessagesListViewModel CreateViewModelWithDummyMessages()
		{
			var listViewModel = new BuildMessagesListViewModel();
			listViewModel.AddMessage("A simple build warning".AsBuildTestWarning());
			listViewModel.AddMessage("A simple build error".AsBuildTestError());
			listViewModel.AddMessage("A second simple build error".AsBuildTestError());
			return listViewModel;
		}

		[Test, STAThread, Ignore]
		public void ShowViewWithWithDummyMessages()
		{
			var listViewModel = CreateViewModelWithDummyMessages();
			var window = CreateVerifiableWindowFromViewModel(listViewModel);
			listViewModel.AddMessage("This error was added later".AsBuildTestError());
			window.ShowDialog();
		}
	}
}