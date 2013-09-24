using System;
using System.Collections.ObjectModel;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Editor.Core;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D.Sprites;
using DeltaEngine.Rendering3D.Cameras;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

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
			RefreshContentList();
		}

		private readonly Service service;

		private void SetMessenger()
		{
			Messenger.Default.Register<ContentIconAndName[]>(this, "DeleteContent",
				DeleteContentFromList);
			Messenger.Default.Register<bool>(this, "DeleteContent", DeleteContentFromList);
		}

		public void DeleteContentFromList(ContentIconAndName[] contentList)
		{
			foreach (var content in contentList)
				service.DeleteContent(content.Name);
		}

		public void DeleteContentFromList(bool deleteSubContent)
		{
			service.DeleteContent(selectedContent.Name, deleteSubContent);
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
				AddNewContent(content);
			RaisePropertyChanged("ContentList");
		}

		private void AddNewContent(string contentName)
		{
			var contentType = service.GetTypeOfContent(contentName);
			ContentList.Add(new ContentIconAndName(GetContentTypeIcon(contentType), contentName));
		}

		private static string GetContentTypeIcon(ContentType? type)
		{
			var iconFileName = type == ContentType.Font
				? "Xml.png" : type == ContentType.ParticleEmitter ? "ParticleEmitter.png" : type + ".png";
			return Path.Combine(ContentTypeFolder, iconFileName);
		}

		public ObservableCollection<ContentIconAndName> ContentList { get; set; }
		private const string ContentTypeFolder = "ContentTypes";

		public ContentIconAndName SelectedContent
		{
			get { return selectedContent; }
			set
			{
				selectedContent = value;
				ClearEntitiesExceptCamera();
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
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
			}
		}

		private readonly ContentViewer contentViewer = new ContentViewer();

		public string SelectedBackgroundImage
		{
			get { return selectedBackgroundImage; }
			set
			{
				selectedBackgroundImage = value;
				ClearEntitiesExceptCamera();
				CreatePreviewerForSelectedContent();
				DrawBackground();
			}
		}

		private string selectedBackgroundImage;

		private static void ClearEntitiesExceptCamera()
		{
			var entities = EntitiesRunner.Current.GetAllEntities();
			foreach (var entity in entities)
				if (entity.GetType() != typeof(Camera))
					entity.IsActive = false;
		}

		private void DrawBackground()
		{
			if (selectedBackgroundImage == null || selectedBackgroundImage == "None")
				return;
			var background = new Sprite(new Material(Shader.Position2DUv, selectedBackgroundImage),
				new Rectangle(Vector2D.Zero, Size.One));
			background.RenderLayer = -100;
		}
	}
}