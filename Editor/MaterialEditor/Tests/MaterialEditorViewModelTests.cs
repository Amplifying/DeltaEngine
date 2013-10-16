using DeltaEngine.Content;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.MaterialEditor.Tests
{
	public class MaterialEditorViewModelTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			materialEditor = new MaterialEditorViewModel(new MockService("TestUser", "MaterialTests"));
		}

		private MaterialEditorViewModel materialEditor;

		[Test]
		public void GetDefaultVariables()
		{
			Assert.AreEqual("PixelBased", materialEditor.SelectedRenderSize);
			Assert.AreEqual("Normal", materialEditor.SelectedBlendMode);
		}

		[Test]
		public void SaveMaterialFromImage()
		{
			materialEditor.Save();
			materialEditor.SelectedImage = "DeltaEngineLogo";
			materialEditor.MaterialName = "NewMaterial";
			materialEditor.Save();
		}

		[Test]
		public void SaveMaterialFromAnimation()
		{
			materialEditor.Save();
			materialEditor.SelectedAnimation = "NewImageAnimation";
			materialEditor.MaterialName = "NewMaterial";
			materialEditor.Save();
		}

		[Test]
		public void LoadInMaterialWithAnimation()
		{
			materialEditor.MaterialName = "NewMaterialImageAnimation";
		}

		[Test]
		public void LoadInMaterialWithSpriteSheetAnimation()
		{
			materialEditor.MaterialName = "NewMaterialSpriteSheetAnimation";
		}
	}
}