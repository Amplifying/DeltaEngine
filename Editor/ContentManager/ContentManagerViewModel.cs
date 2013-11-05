using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Editor.Core;
using DeltaEngine.Input;
using DeltaEngine.Rendering2D;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using Color = System.Windows.Media.Color;
using Size = DeltaEngine.Datatypes.Size;
using Trigger = DeltaEngine.Commands.Trigger;

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
			StartPreviewList = new List<Entity2D>();
			selectedContentList = new List<string>();
			queuedContent = new List<string>();
			if (!IsLoggedInAlready())
				return;
			RefreshContentList();
			IsShowingStartContent = true;
			Trigger trigger = new MouseButtonTrigger();
			trigger.AddTag("ViewControl");
			var mouseCommand = new Command(position => CheckMousePosition(position)).Add(trigger);
			mouseCommand.AddTag("ViewControl");
			ShowingContentManager = true;
		}

		private bool IsLoggedInAlready()
		{
			return !string.IsNullOrEmpty(service.UserName);
		}

		private void CheckMousePosition(Vector2D position)
		{
			if (!IsShowingStartContent || !ShowingContentManager)
				return;
			foreach (var entity2D in StartPreviewList)
				if (entity2D.DrawArea.Contains(position))
					foreach (var content in ContentList)
						if (entity2D.Get<Material>().Animation != null &&
							content.Name == entity2D.Get<Material>().Animation.Name)
							SelectedContent = content;
						else if (content.Name == entity2D.Get<Material>().DiffuseMap.Name)
							SelectedContent = content;
			if (selectedContent != null)
				IsShowingStartContent = false;
		}

		private readonly Service service;
		private readonly List<Entity2D> StartPreviewList;
		public readonly List<string> selectedContentList;
		private bool IsShowingStartContent;

		private void SetMessenger()
		{
			Messenger.Default.Register<string>(this, "DeleteContent", DeleteContentFromList);
			Messenger.Default.Register<bool>(this, "DeleteContent", DeleteContentFromList);
			Messenger.Default.Register<string>(this, "AddToSelection", AddContentToSelection);
			Messenger.Default.Register<string>(this, "AddMultipleContentToSelection",
				AddMultipleContentToSelection);
			Messenger.Default.Register<string>(this, "ClearList", ClearSelectionList);
			Messenger.Default.Register<string>(this, "OpenFileExplorerToAddNewContent",
				OpenFileExplorerToAddNewContent);
			Messenger.Default.Register<string>(this, "NoLongerSelectContentManager",
				NoLongerSelectContentManager);
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
			if (selectedContentList.Contains(contentName))
				foreach (var contentIconAndName in ContentList)
					if (contentIconAndName.Name == contentName)
					{
						contentIconAndName.Brush = new SolidColorBrush(Color.FromArgb(0, 175, 175, 175));
						selectedContentList.Remove(contentName);
						return;
					}
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
			int indexOfLastContent =
				ContentList.IndexOf(
					ContentList.FirstOrDefault(content => content.Name == lastSelectedContent));
			var indexOfNewContent =
				ContentList.IndexOf(ContentList.FirstOrDefault(content => content.Name == contentName));
			if (indexOfLastContent < indexOfNewContent)
				for (int i = 0; indexOfLastContent + i < indexOfNewContent; i++)
					SelectNewContent(indexOfLastContent, i);
			else if (indexOfNewContent < indexOfLastContent)
				for (int i = 0; indexOfNewContent + i < indexOfLastContent; i++)
					SelectNewContent(indexOfNewContent, i);
		}

		private void SelectNewContent(int indexOfLastContent, int i)
		{
			ContentList[indexOfLastContent + i].Brush =
				new SolidColorBrush(Color.FromArgb(255, 195, 195, 195));
			selectedContentList.Add(ContentList[indexOfLastContent + i].Name);
		}

		private string lastSelectedContent;

		public void ClearSelectionList(string obj)
		{
			selectedContentList.Clear();
			foreach (var contentIconAndName in ContentList)
				contentIconAndName.Brush = new SolidColorBrush(Color.FromArgb(0, 175, 175, 175));
			RaisePropertyChanged("ContentList");
		}

		public void OpenFileExplorerToAddNewContent(string obj)
		{
			var dialog = new OpenFileDialog { Multiselect = true };
			dialog.ShowDialog(Application.Current.MainWindow);
			files = dialog.FileNames;
			if (isUploadingContent)
			{
				foreach (var file in files)
					queuedContent.Add(file);
				return;
			}
			UploadToOnlineService(files[0]);
			isUploadingContent = true;
			contentUploadIndex++;
			service.ContentUpdated += UploadNextFile;
		}

		private string[] files;
		private int contentUploadIndex;
		private bool isUploadingContent;
		private readonly List<string> queuedContent;

		private void UploadNextFile(ContentType arg1, string arg2)
		{
			if (files.Length < contentUploadIndex + 1)
			{
				contentUploadIndex = 0;
				if (queuedContent.Count == 0)
				{
					service.ContentUpdated -= UploadNextFile;
					isUploadingContent = false;
				}
				else
				{
					files = queuedContent.ToArray();
					queuedContent.Clear();
				}
			}
			else
			{
				UploadToOnlineService(files[contentUploadIndex]);
				contentUploadIndex++;
			}
		}

		public void UploadToOnlineService(string contentFilePath)
		{
			byte[] bytes;
			try
			{
				bytes = File.ReadAllBytes(contentFilePath);
			}
			catch (Exception)
			{
				Logger.Warning("Unable to read bytes for uploading to the server : " +
					Path.GetFileName(contentFilePath));
				return;
			}
			if (bytes.Length > MaximumFileSize)
			{
				Logger.Warning("The file you added is too large, the maximum file size is 16MB");
				return;
			}
			var fileNameAndBytes = new Dictionary<string, byte[]>();
			fileNameAndBytes.Add(Path.GetFileName(contentFilePath), bytes);
			var metaDataCreator = new ContentMetaDataCreator();
			var contentMetaData = metaDataCreator.CreateMetaDataFromFile(contentFilePath);
			service.UploadContent(contentMetaData, fileNameAndBytes);
		}

		private const int MaximumFileSize = 16777216;

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
			IsShowingStartContent = true;
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
			string iconFileName;
			if (type == ContentType.Font)
				iconFileName = "Xml.png";
			else if (type == ContentType.ParticleSystem)
				iconFileName = ContentType.ParticleEmitter + ".png";
			else
				iconFileName = type + ".png";
			return Path.Combine(ContentTypeFolder, iconFileName);
		}

		public ObservableCollection<ContentIconAndName> ContentList { get; set; }
		private const string ContentTypeFolder = "../Images/ContentTypes/";

		public void ShowStartContent()
		{
			if (!ShowingContentManager)
				return;
			ClearEntities();
			int row = 0;
			int column = 0;
			foreach (var content in ContentList)
			{
				if (service.GetTypeOfContent(content.Name) == ContentType.Image ||
					service.GetTypeOfContent(content.Name) == ContentType.SpriteSheetAnimation ||
					service.GetTypeOfContent(content.Name) == ContentType.ImageAnimation)
				{
					if (content.Name.Contains("Font"))
						continue;
					try
					{
						var sprite = new Sprite(content.Name, new Rectangle(0, 0, 1, 1));
						SetDrawAreaPosition(sprite, row, column);
					}
					catch (Exception)
					{
						continue;
					}
				}
				else if (service.GetTypeOfContent(content.Name) == ContentType.Material)
					try
					{
						var material = ContentLoader.Load<Material>(content.Name);
						var sprite = new Sprite(material, new Rectangle(0, 0, 1, 1));
						SetDrawAreaPosition(sprite, row, column);
					}
					catch (Exception)
					{
						continue;
					}
				else
					continue;
				if (UpdateRowAndColumn(ref column, ref row))
					return;
			}
		}

		private bool ShowingContentManager;

		private static bool UpdateRowAndColumn(ref int column, ref int row)
		{
			column++;
			if (column > MaxRows)
			{
				column = 0;
				row++;
			}
			if (row == MaxColumns)
				return true;
			return false;
		}

		private const int MaxRows = 3;
		private const int MaxColumns = 3;

		private void SetDrawAreaPosition(Entity2D entity, int row, int column)
		{
			Size size;
			if (entity.Get<Material>().DiffuseMap != null)
				size = entity.Get<Material>().DiffuseMap.PixelSize;
			else
				size = entity.Get<Material>().Animation.Frames[0].PixelSize;
			if (size.Width > size.Height)
			{
				var aspect = size.Height / size.Width;
				entity.DrawArea = new Rectangle(0.1f + 0.2f * column, 0.25f + 0.2f * row, 0.15f,
					0.15f * aspect);
			}
			else
			{
				var aspect = size.Width / size.Height;
				entity.DrawArea = new Rectangle(0.1f + 0.2f * column, 0.25f + 0.2f * row, 0.15f * aspect,
					0.15f);
			}
			StartPreviewList.Add(entity);
		}

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
			if (service.Viewport == null)
				return;
			service.Viewport.CenterViewOn(Vector2D.Half);
			service.Viewport.ZoomViewTo(1.0f);
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

		private void ClearEntities()
		{
			if (service.Viewport != null)
				service.Viewport.DestroyRenderedEntities();
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

		public void Activate()
		{
			ShowingContentManager = true;
			IsShowingStartContent = true;
			ShowStartContent();
		}

		public void NoLongerSelectContentManager(string obj)
		{
			ShowingContentManager = false;
		}
	}
}