using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Windows;
using DeltaEngine.Content.Online;
using DeltaEngine.Editor.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests
{
	[Category("Slow")]
	internal class ContentManagerTests
	{
		[SetUp]
		public void SetUp()
		{
			contentPath = "Content";
			var fileSystem =
				new MockFileSystem(new Dictionary<string, MockFileData>
				{
					{
						@"Content\DeltaEngineLogo.png",
						new MockFileData(DataToString(Path.Combine(contentPath, @"DeltaEngineLogo.png")))
					},
				});
			var service = new ContentServiceMock(fileSystem);
			contentManager = new ContentManagerViewModel(service);
		}

		private string contentPath;

		private static string DataToString(string path)
		{
			var fileSystem = new FileSystem();
			return fileSystem.File.ReadAllText(path);
		}

		private ContentManagerViewModel contentManager;

		[Test]
		public void CreateNewContentManagerAndContentFolder()
		{
			Assert.AreEqual(0, contentManager.Images.Count);
			Assert.AreEqual(2, contentManager.Projects.Count);
		}

		[Test]
		public void DropImageInContentManager()
		{
			contentManager.SelectedProject = "BreakOut";
			var filePath = new StringCollection();
			filePath.Add(Path.Combine(contentPath, "DeltaEngineLogo.png"));
			var imageObject = new DataObject();
			imageObject.SetFileDropList(filePath);
			contentManager.DropContent(imageObject);
			Assert.AreEqual(2, contentManager.Images.Count);
		}

		[Test]
		public void DropImageInContentManagerThatIsNotDrop()
		{
			var imageObject = new DataObject();
			contentManager.DropContent(imageObject);
			Assert.AreEqual(0, contentManager.Images.Count);
		}

		[Test, Ignore]
		public void DeleteSelectedImage()
		{
			contentManager.SelectedProject = "Content";
			contentManager.SelectedContent = "DeltaEngineLogo.png";
			contentManager.DeleteImageFromList("DeltaEngineLogo.png");
		}

		[Test, Ignore]
		public void EditExistingImage()
		{
			contentManager.SelectedProject = "Content";
			contentManager.SelectedContent = "DeltaEngineLogo.png";
			contentManager.EditViewImage();
		}

		[Test]
		public void GetNonExistingImage()
		{
			contentManager.SelectedProject = "Breakout";
			Assert.Throws<FileNotFoundException>(
				() => contentManager.SelectedContent = "DeltaEngineLogo.png");
		}

		[Test]
		public void AddNewProject()
		{
			contentManager.NewProjectName = "NewProject";
			contentManager.AddNewProject("test2");
			Assert.AreEqual(2, contentManager.Projects.Count);
		}

		[Test, Ignore]
		public void TryToCreateNewProjectWithEmptyTextbox()
		{
			contentManager.NewProjectName = "";
			contentManager.AddNewProject("test2");
			Assert.AreEqual(2, contentManager.Projects.Count);
		}

		[Test]
		public void ChangeSizeOfImage()
		{
			var size = new Size(500, 500);
			contentManager.ChangeImageSize(size);
			Assert.AreEqual(500, contentManager.ImageWidth);
			Assert.AreEqual(500, contentManager.ImageHeight);
		}

		[Test]
		public void SaveImagesAsAnimation()
		{
			var itemList = new List<string>();
			itemList.Add("DeltaEngineLogo.png");
			contentManager.SaveImagesAsAnimation(itemList);
		}
	}
}