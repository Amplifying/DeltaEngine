using System.Windows;
using ApprovalTests.Reporters;
using ApprovalTests.Wpf;
using NUnit.Framework;

namespace DeltaEngine.Editor.EditorPlugin.Tests
{
	[UseReporter(typeof(DiffReporter)), Category("Slow")]
	public class EditorPluginViewTests
	{
		[Test]
		public void CreateWindowWithDefaultButton()
		{
			WpfApprovals.Verify(() => CreateVerifiableWindowWithViewModel(viewModel));
		}

		private static Window CreateVerifiableWindowWithViewModel(EditorPluginViewModel viewModel)
		{
			return new Window { Content = new EditorPluginView(viewModel), Width = 400, Height = 300 };
		}

		private EditorPluginViewModel viewModel;

		[SetUp]
		public void Init()
		{
			viewModel = new EditorPluginViewModel();
		}

		[Test]
		public void ButtonClickChangeContentFromClickMeToClicked()
		{
			viewModel.ClickCommand.Execute(null);
			WpfApprovals.Verify(() => CreateVerifiableWindowWithViewModel(viewModel));
		}
	}
}