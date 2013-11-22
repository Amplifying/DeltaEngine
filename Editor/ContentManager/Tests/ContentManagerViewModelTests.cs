using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Entities;
using DeltaEngine.Input.Mocks;
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
			mockMouse = new MockMouse();
			AdvanceTimeAndUpdateEntities();
		}

		private ContentManagerViewModel contentManagerViewModel;
		private MockMouse mockMouse;

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
			Assert.AreEqual(1, contentManagerViewModel.SelectedContentList.Count);
			contentManagerViewModel.ClearSelectionList("");
			Assert.AreEqual(0, contentManagerViewModel.SelectedContentList.Count);
		}

		[Test]
		public void SelectingASelectedContentWillDeselectIt()
		{
			contentManagerViewModel.AddContentToSelection("MyImage1");
			Assert.AreEqual(1, contentManagerViewModel.SelectedContentList.Count);
			contentManagerViewModel.AddContentToSelection("MyImage1");
			Assert.AreEqual(0, contentManagerViewModel.SelectedContentList.Count);
		}

		[Test]
		public void GetOnlyContentWithLetterT()
		{
			Assert.AreEqual(numberOfContentItemsInList, contentManagerViewModel.ContentList.Count);
			contentManagerViewModel.SearchText = ("T");
			Assert.AreEqual(numberOfContentItemsInListWithLetterT,
				contentManagerViewModel.ContentList.Count);
			Assert.AreEqual("#00FFFFFF", contentManagerViewModel.ContentList[0].Brush.ToString());
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

		[Test]
		public void ClickingWhenNoStartContentIsAvailableWillDoNothing()
		{
			contentManagerViewModel.NoLongerSelectContentManager("");
			var numberOfEntities = EntitiesRunner.Current.GetAllEntities().Count;
			contentManagerViewModel.NoLongerSelectContentManager("");
			contentManagerViewModel.CheckMousePosition(new Vector2D(0.1f, 0.1f));
			Assert.AreEqual(numberOfEntities, EntitiesRunner.Current.GetAllEntities().Count);
		}

		[Test]
		public void ShowStartContentAndSelectAnImage()
		{
			contentManagerViewModel.ContentList.Clear();
			contentManagerViewModel.Activate();
			contentManagerViewModel.AddNewContent("Image");
			contentManagerViewModel.AddNewContent("Material");
			contentManagerViewModel.AddNewContent("Font");
			contentManagerViewModel.AddNewContent("Scene");
			contentManagerViewModel.ShowStartContent();
			Assert.AreEqual(2, contentManagerViewModel.StartPreviewList.Count);
			contentManagerViewModel.CheckMousePosition(new Vector2D(0.11f, 0.26f));
			Assert.AreEqual(((ContentIconAndName)contentManagerViewModel.SelectedContent).Name, "Image");
		}

		[Test]
		public void CannotShowPreviewsIfContentManagerIsNotOPen()
		{
			contentManagerViewModel.ShowingContentManager = false;
			contentManagerViewModel.ShowStartContent();
			Assert.AreEqual(0, contentManagerViewModel.StartPreviewList.Count);
		}

		[Test]
		public void ShowStartContentAndSelectAnAnimation()
		{
			AddBasicContentToList();
			contentManagerViewModel.ShowStartContent();
			Assert.AreEqual(2, contentManagerViewModel.StartPreviewList.Count);
			contentManagerViewModel.CheckMousePosition(new Vector2D(0.11f, 0.26f));
			Assert.AreEqual(((ContentIconAndName)contentManagerViewModel.SelectedContent).Name,
				"ImageAnimation");
		}

		private void AddBasicContentToList()
		{
			contentManagerViewModel.ContentList.Clear();
			contentManagerViewModel.AddNewContent("ImageAnimation");
			contentManagerViewModel.AddNewContent("Material");
		}

		[Test]
		public void SelectContentToDelete()
		{
			AddBasicContentToList();
			contentManagerViewModel.SelectToDelete("Material");
			contentManagerViewModel.SelectToDelete("Material");
			Assert.AreEqual(1, contentManagerViewModel.SelectedContentList.Count);
		}

		[Test]
		public void AddMultipleContentToSelectionFromTopToBotom()
		{
			AddBasicContentToList();
			contentManagerViewModel.AddMultipleContentToSelection("Material");
			contentManagerViewModel.AddMultipleContentToSelection("ImageAnimation");
			Assert.AreEqual(3, contentManagerViewModel.SelectedContentList.Count);
		}

		[Test]
		public void AddMultipleContentToSelectionFromBotomToTop()
		{
			AddBasicContentToList();
			contentManagerViewModel.AddMultipleContentToSelection("ImageAnimation");
			contentManagerViewModel.AddMultipleContentToSelection("Material");
			Assert.AreEqual(3, contentManagerViewModel.SelectedContentList.Count);
		}

		//ncrunch: no coverage start 
		[Test, Ignore]
		public void OpenFileExplorerToAddNewContent()
		{
			AddBasicContentToList();
			contentManagerViewModel.OpenFileExplorerToAddNewContent("");
		} //ncrunch: no coverage end

		[Test]
		public void GetContentTypeIcons()
		{
			var fontIcon = contentManagerViewModel.GetContentTypeIcon(ContentType.Font);
			Assert.AreEqual(ContentTypeFolder + "Xml.png", fontIcon);
			var particleIcon = contentManagerViewModel.GetContentTypeIcon(ContentType.ParticleSystem);
			Assert.AreEqual(ContentTypeFolder + ContentType.ParticleEmitter + ".png", particleIcon);
		}

		private const string ContentTypeFolder = "../Images/ContentTypes/";

		[Test]
		public void StartContentShowsOnly12Previews()
		{
			AddBasicContentToList();
			contentManagerViewModel.AddNewContent("EarthImages");
			for (int i = 0; i < 13; i++)
				contentManagerViewModel.AddNewContent("Material" + i);
			contentManagerViewModel.ShowStartContent();
			Assert.AreEqual(12, contentManagerViewModel.StartPreviewList.Count);
		}
	}
}