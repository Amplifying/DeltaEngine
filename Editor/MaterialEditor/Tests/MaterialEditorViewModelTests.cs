using DeltaEngine.Content;
using DeltaEngine.Core;
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
		public void ChangeBlendModeAndRenderSize()
		{
			materialEditor.SelectedRenderSize = RenderSizeMode.SizeFor1024X768.ToString();
			materialEditor.SelectedBlendMode = BlendMode.Opaque.ToString();
			Assert.AreEqual("SizeFor1024X768", materialEditor.SelectedRenderSize);
			Assert.AreEqual("Opaque", materialEditor.SelectedBlendMode);
		}

		[Test]
		public void SaveMaterialFromImage()
		{
			materialEditor.SelectedImage = "DeltaEngineLogo";
			materialEditor.MaterialName = "NewMaterial";
			materialEditor.Save();
			Assert.AreEqual(1, mockService.NumberOfMessagesSent);
		}

		[Test]
		public void SaveMaterialFromAnimation()
		{
			materialEditor.RefreshOnContentChange();
			materialEditor.Save();
			Assert.AreEqual(0, mockService.NumberOfMessagesSent);
			materialEditor.SelectedAnimation = "ImageAnimation";
			materialEditor.MaterialName = "NewMaterial";
			materialEditor.Save();
			Assert.AreEqual(1, mockService.NumberOfMessagesSent);
		}

		[Test]
		public void LoadInMaterialWithAnimation()
		{
			materialEditor.MaterialName = "NewMaterialImageAnimation";
			materialEditor.LoadMaterial();
			Assert.IsNotNull(materialEditor.NewMaterial);
			Assert.AreEqual(3, materialEditor.NewMaterial.Animation.Frames.Length);
		}

		[Test]
		public void AddMaterialToMaterialList()
		{
			Assert.AreEqual(2, materialEditor.MaterialList.Count);
			materialEditor.RefreshOnAddedContent(ContentType.Material, "NewMaterial");
			Assert.IsFalse(materialEditor.CanSaveMaterial);
			Assert.AreEqual(3, materialEditor.MaterialList.Count);
		}

		[Test]
		public void AddSpriteSheetToSpriteSheetList()
		{
			Assert.AreEqual(6, materialEditor.ImageList.Count);
			materialEditor.RefreshOnAddedContent(ContentType.SpriteSheetAnimation, "NewSpriteSheet");
			Assert.AreEqual(7, materialEditor.ImageList.Count);
		}

		[Test]
		public void AddShaderToShaderList()
		{
			Assert.AreEqual(2, materialEditor.ShaderList.Count);
			materialEditor.RefreshOnAddedContent(ContentType.Shader, "NewShader");
			Assert.AreEqual(3, materialEditor.ShaderList.Count);
		}

		[Test]
		public void LoadInMaterialWithSpriteSheetAnimation()
		{
			materialEditor.MaterialName = "NewMaterialSpriteSheetAnimation";
			materialEditor.LoadMaterial();
			Assert.IsNotNull(materialEditor.NewMaterial);
			Assert.AreEqual(new Size(107, 80), materialEditor.NewMaterial.SpriteSheet.SubImageSize);
		}

		[Test]
		public void ResetListWhenChangingProject()
		{
			materialEditor.Activate();
			Assert.AreEqual(2, materialEditor.MaterialList.Count);
			materialEditor.RefreshOnAddedContent(ContentType.Material, "NewMaterial");
			Assert.AreEqual(3, materialEditor.MaterialList.Count);
			materialEditor.ResetOnProjectChange();
			Assert.AreEqual(2, materialEditor.MaterialList.Count);
			materialEditor.renderExample = null;
			materialEditor.Activate();
			Assert.AreEqual(2, materialEditor.MaterialList.Count);
		}
	}
}