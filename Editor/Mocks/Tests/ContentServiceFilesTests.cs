using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using NUnit.Framework;

namespace DeltaEngine.Editor.Mocks.Tests
{
	internal class ContentServiceFilesTests
	{
		[SetUp]
		public void SetUp()
		{
			var fileSystem =
				new MockFileSystem(new Dictionary<string, MockFileData>
				{
					{
						@"Content\BreakOut\DeltaEngineLogo.png",
						new MockFileData(DataToString(@"Content\DeltaEngineLogo.png"))
					},
				});
			service = new ContentServiceFiles(fileSystem);
		}

		private ContentServiceFiles service;

		private static string DataToString(string path)
		{
			var fileSystem = new FileSystem();
			return fileSystem.File.ReadAllText(path);
		}

		[Test, Category("Slow")]
		public void GetProjects()
		{
			Assert.AreEqual(2, service.GetProjects().Count);
		}

		[Test, Category("Slow")]
		public void GetContent()
		{
			Assert.AreEqual(1, service.GetContentNames("BreakOut").Count);
		}

		[Test, Category("Slow")]
		public void LoadAndAddContent()
		{
			Stream stream = service.LoadContent("BreakOut", "DeltaEngineLogo.png");
			service.AddContent("BreakOut", "DeltaEngineLogo.png", stream);
			Assert.AreEqual(1, service.GetContentNames("BreakOut").Count);
		}

		[Test, Category("Slow")]
		public void RemoveContent()
		{
			service.DeleteContent("BreakOut", "DeltaEngineLogo.png");
			Assert.AreEqual(0, service.GetContentNames("BreakOut").Count);
		}

		[Test, Category("Slow")]
		public void DeleteNonExistingImage()
		{
			service.DeleteContent("BreakOut", "Test.png");
			Assert.AreEqual(1, service.GetContentNames("BreakOut").Count);
		}

		[Test, Category("Slow")]
		public void LoadNonExistingImage()
		{
			Assert.Throws<FileNotFoundException>(
			() => service.LoadContent("BreakOut", "Test.png"));
		}

		[Test, Category("Slow")]
		public void AddSameImageTwice()
		{
			Stream stream = service.LoadContent("BreakOut", "DeltaEngineLogo.png");
			service.AddContent("BreakOut", "DeltaEngineLogo.png", stream);
			service.AddContent("BreakOut", "DeltaEngineLogo.png", stream);
			Assert.AreEqual(1, service.GetContentNames("BreakOut").Count);
		}

		[Test, Category("Slow")]
		public void DeleteWithNoProject()
		{
			service.DeleteContent(null, "test.png");
			Assert.AreEqual(1, service.GetContentNames("BreakOut").Count);
		}
	}
}