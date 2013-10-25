using DeltaEngine.Datatypes;
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
			mockService = new MockService("TestUser", "MaterialTests");
			materialEditor = new MaterialEditorViewModel(mockService);
		}

		private MockService mockService;
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
			materialEditor.SelectedImage = "DeltaEngineLogo";
			materialEditor.MaterialName = "NewMaterial";
			materialEditor.Save();
			Assert.AreEqual(2, mockService.NumberOfMessagesSent);
		}

		[Test]
		public void SaveMaterialFromAnimation()
		{
			materialEditor.Save();
			Assert.AreEqual(0, mockService.NumberOfMessagesSent);
			materialEditor.SelectedAnimation = "ImageAnimation";
			materialEditor.MaterialName = "NewMaterial";
			materialEditor.Save();
			Assert.AreEqual(2, mockService.NumberOfMessagesSent);
		}

		[Test]
		public void LoadInMaterialWithAnimation()
		{
			materialEditor.MaterialName = "NewMaterialImageAnimation";
			Assert.IsNotNull(materialEditor.NewMaterial);
			Assert.AreEqual(3, materialEditor.NewMaterial.Animation.Frames.Length);
		}

		[Test]
		public void LoadInMaterialWithSpriteSheetAnimation()
		{
			materialEditor.MaterialName = "NewMaterialSpriteSheetAnimation";
			Assert.IsNotNull(materialEditor.NewMaterial);
			Assert.AreEqual(new Size(32, 32), materialEditor.NewMaterial.SpriteSheet.SubImageSize);
		}
	}
}