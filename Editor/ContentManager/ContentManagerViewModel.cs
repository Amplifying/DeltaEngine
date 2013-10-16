using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Editor.Core;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Color = System.Windows.Media.Color;

namespace DeltaEngine.Editor.ContentManager
{
	/// <summary>
	/// passes the data used in the ContentManagerView
	/// </summary>
	public sealed class ContentManagerViewModel : ViewModelBase
	{
		public ContentManagerViewModel(Service service)
		{
			this.service = service;
			SetMessenger();
			ContentList = new ObservableCollection<ContentIconAndName>();
			selectedContentList = new List<string>();
			RefreshContentList();
		}

		private readonly Service service;
		public readonly List<string> selectedContentList;

		private void SetMessenger()
		{
			Messenger.Default.Register<string>(this, "DeleteContent", DeleteContentFromList);
			Messenger.Default.Register<bool>(this, "DeleteContent", DeleteContentFromList);
			Messenger.Default.Register<string>(this, "AddToSelection", AddContentToSelection);
			Messenger.Default.Register<string>(this, "AddMultipleContentToSelection",
				AddMultipleContentToSelection);
			Messenger.Default.Register<string>(this, "ClearList", ClearSelectionList);
		}

		public void DeleteContentFromList(string contentList)
		{
			foreach (var contentName in selectedContentList)
				service.DeleteContent(contentName);
		}

		public void DeleteContentFromList(bool deleteSubContent)
		{
			var type = (ContentType)service.GetTypeOfContent(selectedContent.Name);
			if (type == ContentType.ImageAnimation)
				DeleteImageAnimation();
			else if (type == ContentType.SpriteSheetAnimation)
				DeleteSpriteSheetAnimation();
			service.DeleteContent(selectedContent.Name, deleteSubContent);
		}

		private void DeleteImageAnimation()
		{
			var animation = ContentLoader.Load<ImageAnimation>(selectedContent.Name);
			var images = animation.MetaData.Get("ImageNames", "");
			foreach (var image in images.Split(new[] { ',', ' ' }))
				service.DeleteContent(image);
			service.DeleteContent(selectedContent.Name);
		}

		private void DeleteSpriteSheetAnimation()
		{
			var animation = ContentLoader.Load<SpriteSheetAnimation>(selectedContent.Name);
			var image = animation.MetaData.Get("ImageName", "");
			service.DeleteContent(image);
			service.DeleteContent(selectedContent.Name);
		}

		public void AddContentToSelection(string contentName)
		{
			foreach (var contentIconAndName in ContentList)
				if (contentIconAndName.Name == contentName)
					contentIconAndName.Brush = new SolidColorBrush(Color.FromArgb(255, 195, 195, 195));
			selectedContentList.Add(contentName);
			lastSelectedContent = contentName;
			RaisePropertyChanged("ContentList");
		}

		private void AddMultipleContentToSelection(string contentName)
		{
			foreach (var contentIconAndName in ContentList)
				if (contentIconAndName.Name == contentName)
				{
					if (selectedContentList.Count != 0)
						GetContentBetweenLastAndNewContent(contentName);
					contentIconAndName.Brush = new SolidColorBrush(Color.FromArgb(255, 195, 195, 195));
					selectedContentList.Add(contentName);
					lastSelectedContent = contentName;
				}
		}

		private void GetContentBetweenLastAndNewContent(string contentName)
		{
			int indexsOfLastContent =
				ContentList.IndexOf(
					ContentList.FirstOrDefault(content => content.Name == lastSelectedContent));
			var indexsOfNewContent =
				ContentList.IndexOf(ContentList.FirstOrDefault(content => content.Name == contentName));
			if (indexsOfLastContent < indexsOfNewContent)
				for (int i = 0; indexsOfLastContent + i < indexsOfNewContent; i++)
					SelectNewContent(indexsOfLastContent, i);
			else if (indexsOfNewContent < indexsOfLastContent)
				for (int i = 0; indexsOfNewContent + i < indexsOfLastContent; i++)
					SelectNewContent(indexsOfLastContent, i);
		}

		private void SelectNewContent(int indexsOfLastContent, int i)
		{
			ContentList[indexsOfLastContent + i].Brush =
				new SolidColorBrush(Color.FromArgb(255, 195, 195, 195));
			selectedContentList.Add(ContentList[indexsOfLastContent + i].Name);
		}

		private string lastSelectedContent;

		public void ClearSelectionList(string obj)
		{
			selectedContentList.Clear();
			foreach (var contentIconAndName in ContentList)
				contentIconAndName.Brush = new SolidColorBrush(Color.FromArgb(0, 175, 175, 175));
			RaisePropertyChanged("ContentList");
		}

		public bool IsAnimation
		{
			get
			{
				if (selectedContent == null)
					return false;
				var contentType = service.GetTypeOfContent(selectedContent.Name);
				return contentType == ContentType.ImageAnimation ||
					contentType == ContentType.SpriteSheetAnimation || contentType == ContentType.Material;
			}
		}

		public void RefreshContentList()
		{
			ContentList.Clear();
			var foundContent = service.GetAllContentNames();
			foreach (string content in foundContent)
				if (string.IsNullOrEmpty(SearchText))
					AddNewContent(content);
				else if (content.ToLower().Contains(SearchText.ToLower()))
					AddNewContent(content);
			RaisePropertyChanged("ContentList");
		}

		public void AddNewContent(string contentName)
		{
			var contentType = service.GetTypeOfContent(contentName);
			ContentList.Add(new ContentIconAndName(GetContentTypeIcon(contentType), contentName));
		}

		public void AddAnimationAndSubEntries(string contentName, ContentType? contentType)
		{
			var subContent = new ObservableCollection<ContentIconAndName>();
			var animation = ContentLoader.Load<ImageAnimation>(contentName);
			var images = animation.MetaData.Get("ImageNames", "image");
			foreach (var image in images.Split(new[] { ',', ' ' }))
				if (!String.IsNullOrEmpty(image))
					subContent.Add(new ContentIconAndName(GetContentTypeIcon(ContentType.Material), image));
			ContentList.Add(new ContentIconAndName(GetContentTypeIcon(contentType), contentName,
				subContent));
		}

		private static string GetContentTypeIcon(ContentType? type)
		{
			var iconFileName = type == ContentType.Font ? "Xml.png" : type + ".png";
			return Path.Combine(ContentTypeFolder, iconFileName);
		}

		public ObservableCollection<ContentIconAndName> ContentList { get; set; }
		private const string ContentTypeFolder = "../Images/ContentTypes/";

		public Object SelectedContent
		{
			get { return selectedContent; }
			set
			{
				selectedContent = (ContentIconAndName)value;
				ClearEntities();
				CreatePreviewerForSelectedContent();
				DrawBackground();
				RaisePropertyChanged("IsAnimation");
			}
		}

		private ContentIconAndName selectedContent;

		private void CreatePreviewerForSelectedContent()
		{
			if (selectedContent == null)
				return;
			var type = service.GetTypeOfContent(selectedContent.Name);
			if (type == null)
				return;
			try
			{
				contentViewer.View(selectedContent.Name, (ContentType)type);
				if ((ContentType)type == ContentType.ImageAnimation ||
					(ContentType)type == ContentType.SpriteSheetAnimation)
					CanDeleteSubContent = true;
				else
					CanDeleteSubContent = false;
			}
				//ncrunch: no coverage start
			catch (Exception ex)
			{
				Logger.Error(ex);
			}
			//ncrunch: no coverage end
		}

		private readonly ContentViewer contentViewer = new ContentViewer();

		public string SelectedBackgroundImage
		{
			get { return selectedBackgroundImage; }
			set
			{
				selectedBackgroundImage = value;
				ClearEntities();
				CreatePreviewerForSelectedContent();
				DrawBackground();
			}
		}

		private string selectedBackgroundImage;

		private static void ClearEntities()
		{
			var entities = EntitiesRunner.Current.GetAllEntities();
			foreach (var entity in entities)
				entity.IsActive = false;
		}

		private void DrawBackground()
		{
			if (selectedBackgroundImage == null || selectedBackgroundImage == "None")
				return;
			var background = new Sprite(new Material(Shader.Position2DUV, selectedBackgroundImage),
				new Rectangle(Vector2D.Zero, Size.One));
			background.RenderLayer = -100;
		}

		public string SearchText
		{
			get { return searchText; }
			set
			{
				searchText = value;
				RefreshContentList();
			}
		}

		private string searchText { get; set; }
		public bool CanDeleteSubContent { get; set; }
	}
}