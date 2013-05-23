using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Windows;
using DeltaEngine.Editor.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests
{
	internal class ContentManagerTests
	{
		[SetUp]
		public void SetUp()
		{
			var fileSystem =
				new MockFileSystem(new Dictionary<string, MockFileData>
				{
					{
						@"Content\DeltaEngineLogo.png",
						new MockFileData(DataToString(@"Content\DeltaEngineLogo.png"))
					},
				});
			var service = new ContentServiceMock(fileSystem);
			contentManager = new ContentManagerViewModel(service);
		}

		private static string DataToString(string path)
		{
			var fileSystem = new FileSystem();
			return fileSystem.File.ReadAllText(path);
		}

		private ContentManagerViewModel contentManager;

		[Test, Category("Slow")]
		public void CreateNewContentManagerAndContentFolder()
		{
			Assert.AreEqual(0, contentManager.Images.Count);
			Assert.AreEqual(2, contentManager.Projects.Count);
		}

		[Test, Category("Slow")]
		public void DropImageInContentManager()
		{
			contentManager.SelectedProject = "BreakOut";
			var filePath = new StringCollection();
			filePath.Add(@"Content\DeltaEngineLogo.png");
			var imageObject = new DataObject();
			imageObject.SetFileDropList(filePath);
			contentManager.DropContent(imageObject);
			Assert.AreEqual(2, contentManager.Images.Count);
		}

		[Test, Category("Slow")]
		public void DropImageInContentManagerThatIsNotDrop()
		{
			var imageObject = new DataObject();
			contentManager.DropContent(imageObject);
			Assert.AreEqual(0, contentManager.Images.Count);
		}

		[Test, Category("Slow")]
		public void DeleteSelectedImage()
		{
			contentManager.SelectedProject = "Content";
			contentManager.SelectedContent = "DeltaEngineLogo.png";
			contentManager.DeleteImageFromList("DeltaEngineLogo.png");
		}

		[Test, Category("Slow")]
		public void EditExistingImage()
		{
			contentManager.SelectedProject = "Content";
			contentManager.SelectedContent = "DeltaEngineLogo.png";
			contentManager.EditViewImage();
		}

		[Test, Category("Slow")]
		public void GetNonExistingImage()
		{
			contentManager.SelectedProject = "Breakout";
			Assert.Throws<FileNotFoundException>(
				() => contentManager.SelectedContent = "DeltaEngineLogo.png");
		}

		[Test, Category("Slow")]
		public void AddNewProject()
		{
			contentManager.NewProjectName = "NewProject";
			contentManager.AddNewProject("test2");
			Assert.AreEqual(2, contentManager.Projects.Count);
		}
	}
}