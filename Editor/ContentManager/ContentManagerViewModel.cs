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
using DeltaEngine.Rendering2D.Shapes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using Color = System.Windows.Media.Color;
using Size = DeltaEngine.Datatypes.Size;

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
			SelectedContentList = new List<string>();
			new Command(CheckMousePosition).Add(new MouseButtonTrigger());
			ShowingContentManager = true;
			if (IsLoggedInAlready())
				RefreshContentList();
		}

		private bool IsLoggedInAlready()
		{
			return !string.IsNullOrEmpty(service.UserName);
		}

		public void CheckMousePosition(Vector2D position)
		{
			if (!isShowingStartContent || !ShowingContentManager)
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
				isShowingStartContent = false;
		}

		private readonly Service service;
		public readonly List<Entity2D> StartPreviewList;
		public readonly List<string> SelectedContentList;
		private bool isShowingStartContent;

		private void SetMessenger()
		{
			Messenger.Default.Register<string>(this, "DeleteContent", DeleteContentFromList);
			Messenger.Default.Register<bool>(this, "DeleteContent", DeleteContentFromList);
			Messenger.Default.Register<string>(this, "AddToSelection", AddContentToSelection);
			Messenger.Default.Register<string>(this, "SelectToDelete", SelectToDelete);
			Messenger.Default.Register<string>(this, "AddMultipleContentToSelection",
				AddMultipleContentToSelection);
			Messenger.Default.Register<string>(this, "ClearList", ClearSelectionList);
			Messenger.Default.Register<string>(this, "OpenFileExplorerToAddNewContent",
				OpenFileExplorerToAddNewContent);
		}

		public void DeleteContentFromList(string contentList)
		{
			foreach (var contentName in SelectedContentList)
				service.DeleteContent(contentName);
			ClearEntities();
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
			if (SelectedContentList.Contains(contentName))
				foreach (var contentIconAndName in ContentList)
					if (contentIconAndName.Name == contentName)
					{
						contentIconAndName.Brush = new SolidColorBrush(Color.FromArgb(0, 175, 175, 175));
						SelectedContentList.Remove(contentName);
						return;
					}
			foreach (var contentIconAndName in ContentList)
				if (contentIconAndName.Name == contentName)
					contentIconAndName.Brush = new SolidColorBrush(Color.FromArgb(255, 195, 195, 195));
			SelectedContentList.Add(contentName);
			lastSelectedContent = contentName;
			RaisePropertyChanged("ContentList");
		}

		public void SelectToDelete(string contentName)
		{
			if (SelectedContentList.Contains(contentName))
				return;
			SelectedContentList.Clear();
			foreach (var contentIconAndName in ContentList)
				if (contentIconAndName.Name == contentName)
				{
					contentIconAndName.Brush = new SolidColorBrush(Color.FromArgb(255, 195, 195, 195));
					SelectedContentList.Add(contentName);
					lastSelectedContent = contentName;
				}
				else
				{
					contentIconAndName.Brush = new SolidColorBrush(Color.FromArgb(0, 175, 175, 175));
					SelectedContentList.Remove(contentName);
				}
			RaisePropertyChanged("ContentList");
		}

		public void AddMultipleContentToSelection(string contentName)
		{
			foreach (var contentIconAndName in ContentList)
				if (contentIconAndName.Name == contentName)
				{
					if (SelectedContentList.Count != 0)
						GetContentBetweenLastAndNewContent(contentName);
					contentIconAndName.Brush = new SolidColorBrush(Color.FromArgb(255, 195, 195, 195));
					SelectedContentList.Add(contentName);
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
			SelectedContentList.Add(ContentList[indexOfLastContent + i].Name);
		}

		private string lastSelectedContent;

		public void ClearSelectionList(string obj)
		{
			SelectedContentList.Clear();
			foreach (var contentIconAndName in ContentList)
				contentIconAndName.Brush = new SolidColorBrush(Color.FromArgb(0, 175, 175, 175));
			RaisePropertyChanged("ContentList");
		}

		//ncrunch: no coverage start
		public void OpenFileExplorerToAddNewContent(string obj)
		{
			var dialog = new OpenFileDialog { Multiselect = true };
			if ((bool)dialog.ShowDialog(Application.Current.MainWindow))
				service.UploadMutlipleContentToServer(dialog.FileNames);
		} //ncrunch: no coverage end

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
			isShowingStartContent = true;
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

		public string GetContentTypeIcon(ContentType? type)
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
			if (service.Viewport != null)
				service.Viewport.CenterViewOn(Vector2D.Half); //ncrunch: no coverage
			foreach (var content in ContentList)
			{
				if (service.GetTypeOfContent(content.Name) == ContentType.Image ||
					service.GetTypeOfContent(content.Name) == ContentType.SpriteSheetAnimation ||
					service.GetTypeOfContent(content.Name) == ContentType.ImageAnimation)
				{
					if (content.Name.Contains("Font"))
						continue;
					Entity2D entity = null;
					try
					{
						entity = new Sprite(content.Name, new Rectangle(0, 0, 1, 1));
						SetDrawAreaPosition(entity, row, column);
					}
						//ncrunch: no coverage start 
					catch (Exception)
					{
						if (entity != null)
							entity.IsActive = false;
						continue;
					} //ncrunch: no coverage end
				}
				else if (service.GetTypeOfContent(content.Name) == ContentType.Material)
					try
					{
						var material = ContentLoader.Load<Material>(content.Name);
						var sprite = new Sprite(material, new Rectangle(0, 0, 1, 1));
						SetDrawAreaPosition(sprite, row, column);
					}
						//ncrunch: no coverage start 
					catch (Exception)
					{
						continue;
					} //ncrunch: no coverage end
				else
					continue;
				if (UpdateRowAndColumn(ref column, ref row))
					return;
			}
		}

		public bool ShowingContentManager;

		private static bool UpdateRowAndColumn(ref int column, ref int row)
		{
			column++;
			if (column > MaxRows)
			{
				column = 0;
				row++;
			}
			return row == MaxColumns;
		}

		private const int MaxRows = 3;
		private const int MaxColumns = 3;

		private void SetDrawAreaPosition(Entity2D entity, int row, int column)
		{
			Size size;
			if (entity.Get<Material>().DiffuseMap != null)
				size = entity.Get<Material>().DiffuseMap.PixelSize;
			else
				size = entity.Get<Material>().Animation.Frames[0].PixelSize; //ncrunch: no coverage
			if (size.Width > size.Height)
			{
				var aspect = size.Height / size.Width;
				entity.DrawArea =
					Rectangle.FromCenter(new Vector2D(0.175f + 0.2f * column, 0.3f + 0.2f * row),
						new Size(0.15f, 0.15f * aspect));
			}
			else
			{
				var aspect = size.Width / size.Height;
				entity.DrawArea =
					Rectangle.FromCenter(new Vector2D(0.175f + 0.2f * column, 0.3f + 0.2f * row),
						new Size(0.15f * aspect, 0.15f));
			}
			new Line2D(new Vector2D(0.09f + 0.2f * column, 0.215f + 0.2f * row),
				new Vector2D(0.15f + 0.11f + 0.2f * column, 0.215f + 0.2f * row),
				new Datatypes.Color(67, 78, 200));
			new Line2D(new Vector2D(0.09f + 0.2f * column, 0.215f + 0.2f * row),
				new Vector2D(0.09f + 0.2f * column, 0.125f + 0.26f + 0.2f * row),
				new Datatypes.Color(67, 78, 200));
			new Line2D(new Vector2D(0.15f + 0.11f + 0.2f * column, 0.125f + 0.26f + 0.2f * row),
				new Vector2D(0.15f + 0.11f + 0.2f * column, 0.215f + 0.2f * row),
				new Datatypes.Color(67, 78, 200));
			new Line2D(new Vector2D(0.15f + 0.11f + 0.2f * column, 0.125f + 0.26f + 0.2f * row),
				new Vector2D(0.09f + 0.2f * column, 0.125f + 0.26f + 0.2f * row),
				new Datatypes.Color(67, 78, 200));
			StartPreviewList.Add(entity);
		}

		public Object SelectedContent
		{
			get { return selectedContent; }
			set
			{
				isShowingStartContent = false;
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
			//ncrunch: no coverage start
			service.Viewport.CenterViewOn(Vector2D.Half);
			service.Viewport.ZoomViewTo(1.0f);
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

		private void ClearEntities()
		{
			if (service.Viewport != null)
				service.Viewport.DestroyRenderedEntities(); //ncrunch: no coverage
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

		private string searchText;
		public bool CanDeleteSubContent { get; set; }

		public void Activate()
		{
			ShowingContentManager = true;
			isShowingStartContent = true;
			ShowStartContent();
		}

		public void Deactivate()
		{
			ShowingContentManager = false;
		}
	}
}