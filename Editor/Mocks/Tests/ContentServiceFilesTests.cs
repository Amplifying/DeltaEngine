using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using DeltaEngine.Content.Online;
using NUnit.Framework;

namespace DeltaEngine.Editor.Mocks.Tests
{
	[Category("Slow")]
	public class ContentServiceFilesTests
	{
		[SetUp]
		public void SetUp()
		{
			const string ContentPath = "Content";
			var fileSystem =
				new MockFileSystem(new Dictionary<string, MockFileData>
				{
					{
						ContentPath + @"\DeltaEngineLogo.png",
						new MockFileData(DataToString(Path.Combine(ContentPath, "DeltaEngineLogo.png")))
					},
					{
						@"Content\SecondProject\DeltaEngineLogo.png",
						new MockFileData(DataToString(Path.Combine(ContentPath, "DeltaEngineLogo.png")))
					}
				});
			service = new ContentServiceFiles(fileSystem);
		}

		private const string ProjectName = "DeltaEngine.Editor.Mocks.Tests";
		private ContentServiceFiles service;

		private static string DataToString(string path)
		{
			var fileSystem = new FileSystem();
			return fileSystem.File.ReadAllText(path);
		}

		[Test]
		public void GetProjects()
		{
			Assert.AreEqual(2, service.GetProjects().Count);
		}

		[Test]
		public void GetContent()
		{
			Assert.AreEqual(1, service.GetContentNames(ProjectName).Count);
		}

		[Test]
		public void LoadAndAddContent()
		{
			Stream stream = service.LoadContent(ProjectName, "DeltaEngineLogo.png");
			service.AddContent(ProjectName, "DeltaEngineLogo.png", stream);
			Assert.AreEqual(1, service.GetContentNames(ProjectName).Count);
		}

		[Test]
		public void RemoveContent()
		{
			service.DeleteContent(ProjectName, "DeltaEngineLogo.png");
			Assert.AreEqual(0, service.GetContentNames(ProjectName).Count);
		}

		[Test]
		public void DeleteNonExistingImage()
		{
			service.DeleteContent(ProjectName, "Test.png");
			Assert.AreEqual(1, service.GetContentNames(ProjectName).Count);
		}

		[Test]
		public void LoadNonExistingImage()
		{
			Assert.Throws<FileNotFoundException>(() => service.LoadContent(ProjectName, "Test.png"));
		}

		[Test]
		public void AddSameImageTwice()
		{
			Stream stream = service.LoadContent(ProjectName, "DeltaEngineLogo.png");
			service.AddContent(ProjectName, "DeltaEngineLogo.png", stream);
			service.AddContent(ProjectName, "DeltaEngineLogo.png", stream);
			Assert.AreEqual(1, service.GetContentNames(ProjectName).Count);
		}

		[Test]
		public void DeleteWithNoProject()
		{
			service.DeleteContent(null, "test.png");
			Assert.AreEqual(1, service.GetContentNames(ProjectName).Count);
		}

		[Test]
		public void AddANewProject()
		{
			service.AddProject("TestProject");
			Assert.AreEqual(1, service.GetContentNames(ProjectName).Count);
		}

		[Test]
		public void SaveImagesAsAnimation()
		{
			var itemLis = new List<string>();
			itemLis.Add("DeltaEngineLogo.png");
			service.SaveImagesAsAnimation(itemLis, "testAnimation", ProjectName);
		}
	}
}