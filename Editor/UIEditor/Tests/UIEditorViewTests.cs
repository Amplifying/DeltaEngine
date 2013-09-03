using DeltaEngine.Editor.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.UIEditor.Tests
{
	[Category("Slow")]
	public class UIEditorViewTests : TestWithMocksOrVisually 
	{
		[SetUp]
		public void SetUp()
		{
			uiEditor =
				new UIEditorViewModel(new MockService("TestUser",
					"DeltaEngine.Editor.ImageAnimationEditor.Tests"));
		}

		private UIEditorViewModel uiEditor;
	}
}