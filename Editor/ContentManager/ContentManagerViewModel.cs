using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Editor.Core;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering3D;
using DeltaEngine.Rendering3D.Cameras;
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
		private readonly List<string> selectedContentList;

		private void SetMessenger()
		{
			Messenger.Default.Register<string>(this, "DeleteContent", DeleteContentFromList);
			Messenger.Default.Register<bool>(this, "DeleteContent", DeleteContentFromList);
			Messenger.Default.Register<string>(this, "AddToSelection", AddContentToSelection);
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
				ContentLoader.Load<SpriteSheetAnimation>(selectedContent.Name);
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

		private void AddContentToSelection(string contentName)
		{
			foreach (var contentIconAndName in ContentList)
				if (contentIconAndName.Name == contentName)
					contentIconAndName.Brush = new SolidColorBrush(Color.FromArgb(255, 195, 195, 195));
			selectedContentList.Add(contentName);
			RaisePropertyChanged("ContentList");
		}

		private void ClearSelectionList(string obj)
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

		private void AddNewContent(string contentName)
		{
			var contentType = service.GetTypeOfContent(contentName);
			if (contentType == ContentType.Mesh || contentType == ContentType.Geometry)
				return;
			ContentList.Add(new ContentIconAndName(GetContentTypeIcon(contentType), contentName));
		}

		private void AddAnimationAndSubEntries(string contentName, ContentType? contentType)
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

		private void Add3DModelAndSubEntries(string contentName, ContentType? contentType)
		{
			var subContent = new ObservableCollection<ContentIconAndName>();
			var model = ContentLoader.Load<ModelData>(contentName);
			var meshes = model.MetaData.Get("MeshNames", "mesh");
			foreach (var mesh in meshes.Split(new[] { ',', ' ' }))
				if (!String.IsNullOrEmpty(mesh))
					GetSubContentFromMesh(mesh, subContent);
			ContentList.Add(new ContentIconAndName(GetContentTypeIcon(ContentType.Model), contentName,
				subContent));
		}

		private static void GetSubContentFromMesh(string mesh,
			ObservableCollection<ContentIconAndName> subContent)
		{
			var loadedMesh = ContentLoader.Load<Mesh>(mesh);
			subContent.Add(new ContentIconAndName(GetContentTypeIcon(ContentType.Mesh), mesh));
			GetSubEntriesForGeometryAndMaterial(subContent, loadedMesh);
		}

		private static void GetSubEntriesForGeometryAndMaterial(
			ObservableCollection<ContentIconAndName> subContent, Mesh loadedMesh)
		{
			var geometrys = loadedMesh.MetaData.Get("GeometryName", "geometry");
			foreach (var geometry in geometrys.Split(new[] { ',', ' ' }))
				if (!String.IsNullOrEmpty(geometry))
					subContent[subContent.Count - 1].SubContent.Add(
						new ContentIconAndName(GetContentTypeIcon(ContentType.Geometry), geometry));
			var materials = loadedMesh.MetaData.Get("MaterialName", "material");
			foreach (var material in materials.Split(new[] { ',', ' ' }))
				if (!String.IsNullOrEmpty(material))
					subContent[subContent.Count - 1].SubContent.Add(
						new ContentIconAndName(GetContentTypeIcon(ContentType.Material), material));
		}

		private static string GetContentTypeIcon(ContentType? type)
		{
			var iconFileName = type == ContentType.Font
				? "Xml.png" : type == ContentType.ParticleEmitter ? "ParticleEmitter.png" : type + ".png";
			return Path.Combine(ContentTypeFolder, iconFileName);
		}

		public ObservableCollection<ContentIconAndName> ContentList { get; set; }
		private const string ContentTypeFolder = "ContentTypes";

		public Object SelectedContent
		{
			get { return selectedContent; }
			set
			{
				selectedContent = (ContentIconAndName)value;
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
				if ((ContentType)type == ContentType.ImageAnimation ||
					(ContentType)type == ContentType.SpriteSheetAnimation)
					CanDeleteSubContent = true;
				else
					CanDeleteSubContent = false;
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