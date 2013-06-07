using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using NUnit.Framework;

namespace DeltaEngine.Editor.UIEditor.Tests
{
	public class UIEditorViewModelTests
	{
		[SetUp]
		public void CreateUIEditorViewModel()
		{
			var fileSystem =
				new MockFileSystem(new Dictionary<string, MockFileData>
				{
					{
						@"Content\BreakOut\DeltaEngineLogo.png",
						new MockFileData(DataToString(@"Content\DeltaEngineLogo.png"))
					}
				});
			uiEditorViewModel = new UIEditorViewModel(fileSystem);
		}

		private UIEditorViewModel uiEditorViewModel;

		private static string DataToString(string path)
		{
			var fileSystem = new FileSystem();
			return fileSystem.File.ReadAllText(path);
		}

		[Test, Category("Slow")]
		public void CreatNewUIEditorViewModel()
		{
			var newUIEditorViewModel = new UIEditorViewModel(new FileSystem());
			Assert.IsNotNull(newUIEditorViewModel);
		}

		[Test, Category("Slow")]
		public void ChangeProjectName()
		{
			uiEditorViewModel.ProjectName = "BreakOut";
			Assert.AreEqual(1, uiEditorViewModel.IuImagesInList.Count);
			Assert.AreEqual("BreakOut", uiEditorViewModel.ProjectName);
		}

		[Test, Category("Slow")]
		public void AddAnImageToTheGrid()
		{
			AddImageToGrid();
			Assert.AreEqual(1, uiEditorViewModel.UIImages.Count);
		}

		private void AddImageToGrid()
		{
			uiEditorViewModel.ProjectName = "BreakOut";
			uiEditorViewModel.SelectedImageInList = "DeltaEngineLogo.png";
			uiEditorViewModel.AddImage("DeltaEngineLogo.png");
			uiEditorViewModel.SelectedImageInGridIndex = 0;
		}

		[Test, Category("Slow")]
		public void ChangeGridSize()
		{
			uiEditorViewModel.GridWidth = 20;
			uiEditorViewModel.GridHeight = 20;
			Assert.AreEqual(20, uiEditorViewModel.GridWidth);
			Assert.AreEqual(20, uiEditorViewModel.GridHeight);
		}

		[Test, Category("Slow")]
		public void ChangeIsButton()
		{
			AddImageToGrid();
			uiEditorViewModel.ChangeIsButton(true);
			Assert.IsTrue(uiEditorViewModel.UIImages[0].IsButton);
		}

		[Test, Category("Slow")]
		public void ChangeVisualIsButton()
		{
			AddImageToGrid();
			uiEditorViewModel.CheckBoxButton = true;
			uiEditorViewModel.ChangeVisualIsButton("");
			Assert.IsFalse(uiEditorViewModel.CheckBoxButton);
		}

		[Test, Category("Slow")]
		public void ChangeVisualIsButtonWithMoSelectedImage()
		{
			uiEditorViewModel.SelectedImageInGridIndex = -1;
			uiEditorViewModel.ChangeVisualIsButton("");
			Assert.IsFalse(uiEditorViewModel.CheckBoxButton);
		}

		[Test, Category("Slow")]
		public void ChangeGrid()
		{
			uiEditorViewModel.ChangeGridHeight(500);
			uiEditorViewModel.ChangeGridWidth(500);
			uiEditorViewModel.ChangePixelSnapGrid(0);
			Assert.AreEqual(1, uiEditorViewModel.PixelSnapgrid);
		}

		[Test, Category("Slow")]
		public void ChangeLayer()
		{
			AddImageToGrid();
			uiEditorViewModel.ChangeLayer(5);
			Assert.AreEqual(5, uiEditorViewModel.UIImages[0].Layer);
		}

		[Test, Category("Slow")]
		public void ChangeVisualLayer()
		{
			AddImageToGrid();
			uiEditorViewModel.ChangeVisualLayer("DeltaEngineLogo.png");
			uiEditorViewModel.SelectedImageInGridIndex = -1;
			uiEditorViewModel.ChangeVisualLayer("DeltaEngineLogo.png");
			uiEditorViewModel.ChangeLayer(5);
			Assert.AreEqual(10, uiEditorViewModel.CheckLayer);
		}

		[Test, Category("Slow")]
		public void ChangeRotate()
		{
			AddImageToGrid();
			uiEditorViewModel.ChangeRotate(5);
			Assert.AreEqual(5, uiEditorViewModel.UIImages[0].Rotate.Angle);
		}

		[Test, Category("Slow")]
		public void ChangeVisualRotateSlider()
		{
			AddImageToGrid();
			uiEditorViewModel.ChangeVisualRotateSlider("DeltaEngineLogo.png");
			uiEditorViewModel.SelectedImageInGridIndex = -1;
			uiEditorViewModel.ChangeVisualRotateSlider("DeltaEngineLogo.png");
			Assert.AreEqual(0, uiEditorViewModel.Rotate);
		}

		[Test, Category("Slow")]
		public void ChangeScale()
		{
			AddImageToGrid();
			uiEditorViewModel.ChangeScale(1.2f);
			Assert.AreEqual(1.2f, uiEditorViewModel.UIImages[0].Scale.ScaleX);
		}

		[Test, Category("Slow")]
		public void ChangeVisualScaleSlider()
		{
			AddImageToGrid();
			uiEditorViewModel.ChangeVisualScaleSlider("");
			uiEditorViewModel.SelectedImageInGridIndex = -1;
			uiEditorViewModel.ChangeVisualScaleSlider("");
			Assert.AreEqual(1.0f, uiEditorViewModel.UIImages[0].Scale.ScaleX);
		}

		[Test, Category("Slow")]
		public void SaveUI()
		{
			AddImageToGrid();
			uiEditorViewModel.SaveUIToXml(@"Content\BreakOut\TestXml");
			Assert.AreEqual(1, uiEditorViewModel.UIImages.Count);
		}

		[Test, Category("Slow")]
		public void SaveUIAndLoad()
		{
			Assert.AreEqual(0, uiEditorViewModel.UIImages.Count);
			string xmlPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string filePath = Path.Combine(xmlPath, "Content", "TestXml.xml");
			uiEditorViewModel.LoadUiFromXml(filePath);
			Assert.AreEqual(1, uiEditorViewModel.UIImages.Count);
		}
	}
}