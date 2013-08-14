using DeltaEngine.Editor.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Editor.MaterialEditor.Tests
{
	public class MaterialEditorViewModelTests
	{
		[Test]
		public void SetUp()
		{
			materialEditor = new MaterialEditorViewModel(new MockService("TestUser", "MaterialTests"));
		}

		private MaterialEditorViewModel materialEditor;
	}
}