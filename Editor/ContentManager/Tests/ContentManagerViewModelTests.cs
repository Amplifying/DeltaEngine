using System;
using DeltaEngine.Content;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests
{
	public class ContentManagerViewModelTests : TestWithMocksOrVisually
	{
		[TestFixtureSetUp]
		public void SetExpectedContentNumbers()
		{
			numberOfContentItemsInList = Enum.GetNames(typeof(ContentType)).Length * 2;
			numberOfContentItemsInListWithLetterT = 0;
			foreach (var contentType in Enum.GetNames(typeof(ContentType)))
				if (contentType.ToLower().Contains("t"))
					numberOfContentItemsInListWithLetterT += 2;
		}

		private int numberOfContentItemsInList;
		private int numberOfContentItemsInListWithLetterT;

		[SetUp]
		public void CreateContentViewModel()
		{
			contentManagerViewModel = new ContentManagerViewModel(new MockService("User", "LogoApp"));
		}

		private ContentManagerViewModel contentManagerViewModel;

		[Test]
		public void CheckTheTypeOfNoneExistingContent()
		{
			contentManagerViewModel.SelectedContent = null;
			Assert.IsFalse(contentManagerViewModel.IsAnimation);
			contentManagerViewModel.SelectedContent = new ContentIconAndName("Image", "Test1");
			contentManagerViewModel.SelectedContent = new ContentIconAndName("Image", "TestUser");
			Assert.AreEqual("#00FFFFFF",
				((ContentIconAndName)contentManagerViewModel.SelectedContent).Brush.ToString());
			Assert.IsFalse(contentManagerViewModel.IsAnimation);
		}

		[Test]
		public void DrawNoneExistingBackgrounds()
		{
			contentManagerViewModel.SelectedBackgroundImage = null;
			contentManagerViewModel.SelectedBackgroundImage = "Test1";
			contentManagerViewModel.SelectedBackgroundImage = "TestUser";
		}

		[Test]
		public void DeleteNoneExistingContent()
		{
			Assert.AreEqual(numberOfContentItemsInList, contentManagerViewModel.ContentList.Count);
			contentManagerViewModel.AddContentToSelection("MyImage1");
			contentManagerViewModel.DeleteContentFromList("");
			Assert.AreEqual(numberOfContentItemsInList, contentManagerViewModel.ContentList.Count);
			Assert.AreEqual(1, contentManagerViewModel.selectedContentList.Count);
			contentManagerViewModel.ClearSelectionList("");
			Assert.AreEqual(0, contentManagerViewModel.selectedContentList.Count);
		}

		[Test]
		public void GetOnlyContentWithLetterT()
		{
			Assert.AreEqual(numberOfContentItemsInList, contentManagerViewModel.ContentList.Count);
			contentManagerViewModel.SearchText = ("T");
			Assert.AreEqual(numberOfContentItemsInListWithLetterT,
				contentManagerViewModel.ContentList.Count);
		}

		[Test]
		public void GetBackgroundAndSelectedContentShouldBeNullIfNotSet()
		{
			Assert.IsNull(contentManagerViewModel.SelectedContent);
			Assert.IsNull(contentManagerViewModel.SelectedBackgroundImage);
		}

		[Test]
		public void DeleteImageAnimationFromList()
		{
			contentManagerViewModel.ContentList.Add(new ContentIconAndName("ImageAnimation",
				"ImageAnimation"));
			contentManagerViewModel.SelectedContent = new ContentIconAndName("ImageAnimation",
				"ImageAnimation");
			contentManagerViewModel.DeleteContentFromList(true);
		}

		[Test]
		public void DeleteContentFromList()
		{
			contentManagerViewModel.ContentList.Add(new ContentIconAndName("SpriteSheetAnimation",
				"NewSpriteSheetAnimation"));
			contentManagerViewModel.SelectedContent = new ContentIconAndName("SpriteSheetAnimation",
				"NewSpriteSheetAnimation");
			contentManagerViewModel.DeleteContentFromList(true);
		}

		[Test]
		public void AddAnimationAndSubEntries()
		{
			Assert.AreEqual(numberOfContentItemsInList, contentManagerViewModel.ContentList.Count);
			contentManagerViewModel.AddAnimationAndSubEntries("ImageAnimation",
				ContentType.ImageAnimation);
			Assert.AreEqual(numberOfContentItemsInList + 1, contentManagerViewModel.ContentList.Count);
		}
	}
}