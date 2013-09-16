using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Editor.Core;
using DeltaEngine.Entities;
using DeltaEngine.Rendering.Cameras;
using DeltaEngine.Rendering.Sprites;
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
			ContentList = new ObservableCollection<ContentView>();
			RefreshContentList();
		}

		private readonly Service service;

		private void SetMessenger()
		{
			Messenger.Default.Register<IList<ContentView>>(this, "DeleteContent", DeleteContentFromList);
			Messenger.Default.Register<bool>(this, "DeleteContent", DeleteContentFromList);
		}

		public void DeleteContentFromList(IList<ContentView> contentList)
		{
			foreach (var content in contentList)
				service.DeleteContent(content.NewContent.ContentName);
		}

		public void DeleteContentFromList(bool deleteSubContent)
		{
			service.DeleteContent(selectedContent.NewContent.ContentName, deleteSubContent);
		}

		public bool IsAnimation
		{
			get
			{
				if (selectedContent == null)
					return false;
				var contentType = service.GetTypeOfContent(selectedContent.NewContent.ContentName);
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

		private void AddNewContent(string content)
		{
			var newContent = new ContentView();
			var type = service.GetTypeOfContent(content);
			CreateNewContentView(content, type, newContent);
			ContentList.Add(newContent);
		}

		private static void CreateNewContentView(string content, ContentType? type,
			ContentView newContent)
		{
			if (type == ContentType.Font)
				newContent.CreateContentView(Path.Combine(ContentTypeFolder, "Xml.png"), content);
			else if (type == ContentType.Particle2DEmitter)
				newContent.CreateContentView(Path.Combine(ContentTypeFolder, "ParticleEmitter.png"),
					content);
			else
				newContent.CreateContentView(Path.Combine(ContentTypeFolder, type + ".png"), content);
		}

		public ObservableCollection<ContentView> ContentList { get; set; }
		private const string ContentTypeFolder = "ContentTypes";

		public ContentView SelectedContent
		{
			get { return selectedContent; }
			set
			{
				selectedContent = value;
				ClearEntitiesExceptCamera();
				CheckTypeOfContent();
				DrawBackground();
				RaisePropertyChanged("IsAnimation");
			}
		}
		private ContentView selectedContent;

		private void CheckTypeOfContent()
		{
			if (selectedContent == null)
				return;
			var type = service.GetTypeOfContent(selectedContent.NewContent.ContentName);
			if (type == null)
				return;
			try
			{
				new ContentViewer().Viewer(selectedContent.NewContent.ContentName, service.ProjectName,
					(ContentType)type);
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
			}
		}

		public string SelectedBackgroundImage
		{
			get { return selectedBackgroundImage; }
			set
			{
				selectedBackgroundImage = value;
				ClearEntitiesExceptCamera();
				CheckTypeOfContent();
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
				new Rectangle(Point.Zero, Size.One));
			background.RenderLayer = -100;
		}
	}
}