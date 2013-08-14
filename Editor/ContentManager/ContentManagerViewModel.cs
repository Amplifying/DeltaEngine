using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Editor.Core;
using DeltaEngine.Entities;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Rendering.Sprites;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Point = DeltaEngine.Datatypes.Point;
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
			metaDataCreator = new ContentMetaDataCreator(service);
			SetMessenger();
			ContentList = new ObservableCollection<string>();
			BackgroundImageList = new ObservableCollection<string>();
			BackgroundImageList.Add("None");
			RefreshContentList();
			RefreshBackgroundImageList();
		}

		private readonly Service service;
		private readonly ContentMetaDataCreator metaDataCreator;

		private void SetMessenger()
		{
			Messenger.Default.Register<string>(this, "DeletingContent", DeleteContentFromList);
			Messenger.Default.Register<IDataObject>(this, "AddContent", DropContent);
		}

		public void DeleteContentFromList(string msg)
		{
			service.DeleteContent(selectedContent);
		}

		public void DropContent(IDataObject dropObject)
		{
			if (!IsFile(dropObject))
				return;
			var files = (string[])dropObject.GetData(DataFormats.FileDrop);
			foreach (var file in files)
				UploadToOnlineService(file);
		}

		private static bool IsFile(IDataObject dropObject)
		{
			return dropObject.GetDataPresent(DataFormats.FileDrop);
		}

		private void UploadToOnlineService(string contentFilePath)
		{
			var bytes = File.ReadAllBytes(contentFilePath);
			if (bytes.Length > MaximumFileSize)
			{
				Logger.Warning("The file you wanted to add is too large, the maximum filesize is 16MB");
				return;
			}
			var fileNameAndBytes = new Dictionary<string, byte[]>();
			fileNameAndBytes.Add(Path.GetFileName(contentFilePath), bytes);
			var contentMetaData = metaDataCreator.CreateMetaDataFromFile(contentFilePath);
			if (ContentLoader.Exists(Path.GetFileName(contentFilePath)))
				service.DeleteContent(Path.GetFileName(contentFilePath));
			service.UploadContent(contentMetaData, fileNameAndBytes);
		}

		private const int MaximumFileSize = 16777216;

		public void RefreshContentList()
		{
			ContentList.Clear();
			var foundContent = service.GetAllContentNames();
			foreach (string content in foundContent)
				ContentList.Add(content);
			RaisePropertyChanged("ContentList");
		}

		public ObservableCollection<string> ContentList { get; set; }

		private void RefreshBackgroundImageList()
		{
			BackgroundImageList.Clear();
			var foundContent = service.GetAllContentNamesByType(ContentType.Image);
			foreach (string content in foundContent)
				BackgroundImageList.Add(content);
			RaisePropertyChanged("BackgroundImageList");
		}

		public ObservableCollection<string> BackgroundImageList { get; set; }

		public string SelectedContent
		{
			get { return selectedContent; }
			set
			{
				selectedContent = value;
				ClearEntitiesExceptCamera();
				CheckTypeOfContent();
				DrawBackground();
			}
		}
		private string selectedContent;

		private void CheckTypeOfContent()
		{
			if (selectedContent == null)
				return;
			var type = service.GetTypeOfContent(selectedContent);
			if (type == null)
				return;
			new ContentViewer().Viewer(selectedContent, (ContentType)type);
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