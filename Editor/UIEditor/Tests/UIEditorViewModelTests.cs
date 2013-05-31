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
			uIEditorViewModel = new UIEditorViewModel(fileSystem);
		}

		private UIEditorViewModel uIEditorViewModel;

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
			uIEditorViewModel.ProjectName = "BreakOut";
			Assert.AreEqual(1, uIEditorViewModel.IuImagesInList.Count);
			Assert.AreEqual("BreakOut", uIEditorViewModel.ProjectName);
		}

		[Test, Category("Slow")]
		public void AddAnImageToTheGrid()
		{
			AddImageToGrid();
			Assert.AreEqual(1, uIEditorViewModel.UIImages.Count);
		}

		private void AddImageToGrid()
		{
			uIEditorViewModel.ProjectName = "BreakOut";
			uIEditorViewModel.SelectedImageInList = "DeltaEngineLogo.png";
			uIEditorViewModel.AddImage("DeltaEngineLogo.png");
			uIEditorViewModel.SelectedImageInGridIndex = 0;
		}

		[Test, Category("Slow")]
		public void ChangeGridSize()
		{
			uIEditorViewModel.GridWidth = 20;
			uIEditorViewModel.GridHeight = 20;
			Assert.AreEqual(20, uIEditorViewModel.GridWidth);
			Assert.AreEqual(20, uIEditorViewModel.GridHeight);
		}

		//[Test, Category("Slow")]
		//public void MoveImage()
		//{
		//	AddImageToGrid();
		//	uIEditorViewModel.SelectedImage = uIEditorViewModel.UIImages[0];
		//	Messenger.Default.Send(new Point(20, 20), "MouseMove");
		//	Messenger.Default.Send(new Point(0.5f, 0.5f), "LeftMouseDown");
		//	Messenger.Default.Send(new Point(30, 30), "MouseMove");
		//	Messenger.Default.Send(new Point(30, 30), "LeftMouseUp");
		//	Assert.AreEqual(30, uIEditorViewModel.UIImages[0].X);
		//	Assert.AreEqual(30, uIEditorViewModel.UIImages[0].Y);
		//}

		[Test, Category("Slow")]
		public void ChangeIsButton()
		{
			AddImageToGrid();
			uIEditorViewModel.ChangeIsButton(true);
			Assert.IsTrue(uIEditorViewModel.UIImages[0].IsButton);
		}

		[Test, Category("Slow")]
		public void ChangeVisualIsButton()
		{
			AddImageToGrid();
			uIEditorViewModel.CheckBoxButton = true;
			uIEditorViewModel.ChangeVisualIsButton("");
			Assert.IsFalse(uIEditorViewModel.CheckBoxButton);
		}

		[Test, Category("Slow")]
		public void ChangeVisualIsButtonWithMoSelectedImage()
		{
			uIEditorViewModel.SelectedImageInGridIndex = -1;
			uIEditorViewModel.ChangeVisualIsButton("");
			Assert.IsFalse(uIEditorViewModel.CheckBoxButton);
		}

		[Test, Category("Slow")]
		public void ChangeGrid()
		{
			uIEditorViewModel.ChangeGridHeight(500);
			uIEditorViewModel.ChangeGridWidth(500);
			uIEditorViewModel.ChangePixelSnapGrid(0);
			Assert.AreEqual(1, uIEditorViewModel.PixelSnapgrid);
		}

		[Test, Category("Slow")]
		public void ChangeLayer()
		{
			AddImageToGrid();
			uIEditorViewModel.ChangeLayer(5);
			Assert.AreEqual(5, uIEditorViewModel.UIImages[0].Layer);
		}

		[Test, Category("Slow")]
		public void ChangeVisualLayer()
		{
			AddImageToGrid();
			uIEditorViewModel.ChangeVisualLayer("DeltaEngineLogo.png");
			uIEditorViewModel.SelectedImageInGridIndex = -1;
			uIEditorViewModel.ChangeVisualLayer("DeltaEngineLogo.png");
			uIEditorViewModel.ChangeLayer(5);
			Assert.AreEqual(10, uIEditorViewModel.CheckLayer);
		}

		[Test, Category("Slow")]
		public void ChangeRotate()
		{
			AddImageToGrid();
			uIEditorViewModel.ChangeRotate(5);
			Assert.AreEqual(5, uIEditorViewModel.UIImages[0].Rotate.Angle);
		}

		[Test, Category("Slow")]
		public void ChangeVisualRotateSlider()
		{
			AddImageToGrid();
			uIEditorViewModel.ChangeVisualRotateSlider("DeltaEngineLogo.png");
			uIEditorViewModel.SelectedImageInGridIndex = -1;
			uIEditorViewModel.ChangeVisualRotateSlider("DeltaEngineLogo.png");
			Assert.AreEqual(0, uIEditorViewModel.Rotate);
		}

		[Test, Category("Slow")]
		public void ChangeScale()
		{
			AddImageToGrid();
			uIEditorViewModel.ChangeScale(1.2f);
			Assert.AreEqual(1.2f, uIEditorViewModel.UIImages[0].Scale.ScaleX);
		}

		[Test, Category("Slow")]
		public void ChangeVisualScaleSlider()
		{
			AddImageToGrid();
			uIEditorViewModel.ChangeVisualScaleSlider("");
			uIEditorViewModel.SelectedImageInGridIndex = -1;
			uIEditorViewModel.ChangeVisualScaleSlider("");
			Assert.AreEqual(1.0f, uIEditorViewModel.UIImages[0].Scale.ScaleX);
		}

		[Test, Category("Slow")]
		public void SaveUI()
		{
			AddImageToGrid();
			uIEditorViewModel.SaveUIToXml(@"Content\BreakOut\TestXml");
			Assert.AreEqual(1, uIEditorViewModel.UIImages.Count);
		}

		[Test, Category("Slow")]
		public void SaveUIAndLoad()
		{
			Assert.AreEqual(0, uIEditorViewModel.UIImages.Count);
			string xmlPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string filePath = Path.Combine(xmlPath, "Content", "TestXml.xml");
			uIEditorViewModel.LoadUiFromXml(filePath);
			Assert.AreEqual(1, uIEditorViewModel.UIImages.Count);
		}
	}
}