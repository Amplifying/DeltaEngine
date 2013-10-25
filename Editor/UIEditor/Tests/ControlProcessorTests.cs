using DeltaEngine.Editor.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.UIEditor.Tests
{
	public class ControlProcessorTests : TestWithMocksOrVisually
	{
		[Test]
		public void Line2DArrayShouldHaveFourElements()
		{
			var control = new ControlProcessor(new UIEditorViewModel(new MockService("", "")));
			Assert.AreEqual(4, control.OutLines.Length);
		}
	}
}