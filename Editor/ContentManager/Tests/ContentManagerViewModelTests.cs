using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Windows;
using DeltaEngine.Platforms;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests
{
	public class ContentManagerViewModelTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreateContentViewModel()
		{
			var fileSystem =
				new MockFileSystem(new Dictionary<string, MockFileData>
				{
					{ @"Content\DeltaEngineLogo.png", new MockFileData(DataToString(@"DeltaEngineLogo.png")) },
					{
						@"Content\CtCreepsWoodScarecrowDiff.png",
						new MockFileData(DataToString(@"CtCreepsWoodScarecrowDiff.png"))
					},
					{
						@"Content\CtCreepsWoodScarecrow.DeltaMesh",
						new MockFileData(DataToString(@"CtCreepsWoodScarecrow.DeltaMesh"))
					},
				});
			//contentManagerViewModel = new ContentManagerViewModel(new OnlineService(), Resolve<ScreenSpace>());
		}

		private static string DataToString(string path)
		{
			var fileSystem = new FileSystem();
			return fileSystem.File.ReadAllText(path);
		}

		//private ContentManagerViewModel contentManagerViewModel;

		[Test]
		public void DropImageInContentManager()
		{
			var filePath = new StringCollection();
			filePath.Add("DeltaEngineLogo.png");
			var imageObject = new DataObject();
			imageObject.SetFileDropList(filePath);
			//contentManagerViewModel.DropContent(imageObject);
			//Assert.AreEqual(1, contentManagerViewModel.ContentList.Count);
		}

		[Test]
		public void Drop3DModelInContentManager()
		{
			var filePath = new StringCollection();
			filePath.Add("CtCreepsWoodScarecrow.DeltaMesh");
			var modelObject = new DataObject();
			modelObject.SetFileDropList(filePath);
			//contentManagerViewModel.DropContent(modelObject);
			//Assert.AreEqual(1, contentManagerViewModel.ContentList.Count);
		}
	}
}