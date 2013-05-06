using NUnit.Framework;

namespace DeltaEngine.Editor.EditorPlugin.Tests
{
	public class EditorPluginViewModelTests
	{
		[Test]
		public void ClickChangesButtonText()
		{
			var viewModel = new EditorPluginViewModel();
			Assert.AreEqual("Click Me", viewModel.Button.Text);
			viewModel.ClickCommand.Execute(null);
			Assert.AreEqual("Clicked", viewModel.Button.Text);
		}
	}
}