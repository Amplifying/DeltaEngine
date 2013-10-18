using DeltaEngine.Editor.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.UIEditor.Tests
{
	public class ControlProcessorTests : TestWithMocksOrVisually
	{
		[SetUp, CloseAfterFirstFrame]
		public void CanCreateControlProcessor()
		{
			controlProcessor =
				new ControlProcessor(
					new UIEditorViewModel(new MockService("TestUser", "NewProjectWithoutContent")));
		}

		private ControlProcessor controlProcessor;

		[Test]
		public void Line2DArrayShouldHaveFourElements()
		{
			Assert.AreEqual(4, controlProcessor.outLines.Length);
		}
	}
}