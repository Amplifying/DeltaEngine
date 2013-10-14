using DeltaEngine.Content;
using DeltaEngine.Content.Mocks;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.MaterialEditor.Tests
{
	public class MaterialEditorViewModelTests : TestWithMocksOrVisually
	{
		[Test]
		public void SetUp()
		{
			materialEditor = new MaterialEditorViewModel(new MockService("TestUser", "MaterialTests"));
		}

		private MaterialEditorViewModel materialEditor;
	}
}