using System.Collections.Generic;
using System.Collections.ObjectModel;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager;
using DeltaEngine.Editor.Core;
using DeltaEngine.Input;
using DeltaEngine.Rendering.Sprites;
using DeltaEngine.Scenes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.UIEditor
{
	public class UIEditorViewModel : ViewModelBase
	{
		public UIEditorViewModel(Service service)
		{
			ContentImageListList = new ObservableCollection<string>();
			UIImagesInList = new ObservableCollection<string>();
			this.service = service;
			scene = new Scene();
			FillContentImageList();
			SetMouseCommands();
			Messenger.Default.Register<string>(this, "AddImage", AddImage);
			Messenger.Default.Register<string>(this, "RemoveSprite", RemoveSprite);
			Messenger.Default.Register<string>(this, "SaveUI", SaveUI);
		}

		private readonly Service service;
		public ObservableCollection<string> ContentImageListList { get; private set; }
		public ObservableCollection<string> UIImagesInList { get; private set; }
		public string ContentPath { get; private set; }
		public Scene scene;

		private void FillContentImageList()
		{
			var imageList = service.GetAllContentNamesByType(ContentType.Image);
			foreach (var image in imageList)
				ContentImageListList.Add(image);
		}

		private void SetMouseCommands()
		{
			new Command(SetSelectedImage).Add(new MouseButtonTrigger());
			new Command(MoveImage).Add(new MousePositionTrigger(MouseButton.Left, State.Pressed));
			new Command(position => lastMousePosition = position).Add(
				new MouseButtonTrigger(MouseButton.Middle));
			new Command(ScaleImage).Add(new MousePositionTrigger(MouseButton.Middle, State.Pressed));
		}

		private void SetSelectedImage(Point mousePosition)
		{
			lastMousePosition = mousePosition;
			foreach (Sprite sprite in scene.Controls)
				if (sprite.DrawArea.Contains(mousePosition))
				{
					SpriteListIndex = scene.Controls.IndexOf(sprite);
					SelectedSprite = sprite;
					SelectedSpriteNameInList = sprite.Material.DiffuseMap.Name;
				}
			RaisePropertyChanged("SelectedSpriteNameInList");
		}

		private void MoveImage(Point mousePosition)
		{
			if (SelectedSprite == null)
				return;
			var relativePosition = mousePosition - lastMousePosition;
			lastMousePosition = mousePosition;
			SelectedSprite.Center += relativePosition;
		}

		private void ScaleImage(Point mousePosition)
		{
			if (SelectedSprite == null)
				return;
			var relativePosition = mousePosition - lastMousePosition;
			lastMousePosition = mousePosition;
			SelectedSprite.Size =
				new Size(SelectedSprite.Size.Width + (SelectedSprite.Size.Width * relativePosition.Y),
					SelectedSprite.Size.Height + (SelectedSprite.Size.Height * relativePosition.Y));
		}

		private Point lastMousePosition = Point.Unused;
		public Sprite SelectedSprite { get; set; }

		public void AddImage(string image)
		{
			AddNewImageToList();
			RaisePropertyChanged("UIImages");
			RaisePropertyChanged("ImageInGridList");
		}

		public string SelectedImageInList { get; set; }

		public void RemoveSprite(string obj)
		{
			if (string.IsNullOrEmpty(SelectedSpriteNameInList) || SelectedSprite == null)
				return;
			UIImagesInList.Remove(SelectedSpriteNameInList);
			scene.Controls.Remove(SelectedSprite);
			SelectedSprite.IsActive = false;
		}

		private void SaveUI(string obj)
		{
			if (scene.Controls.Count == 0 || string.IsNullOrEmpty(UIName))
				return;
			var fileNameAndBytes = new Dictionary<string, byte[]>();
			var bytes = BinaryDataExtensions.ToByteArrayWithTypeInformation(scene);
			fileNameAndBytes.Add(UIName + ".deltaUI", bytes);
			var metaDataCreator = new ContentMetaDataCreator(service);
			ContentMetaData contentMetaData = metaDataCreator.CreateMetaDataFromUI(UIName, bytes);
			service.UploadContent(contentMetaData, fileNameAndBytes);
		}

		public string UIName { get; set; }

		public void AddNewImageToList()
		{
			var customImage = ContentLoader.Create<Image>(new ImageCreationData(new Size(8, 8)));
			var colors = new Color[8 * 8];
			for (int i = 0; i < 8 * 8; i++)
				colors[i] = Color.Purple;
			customImage.Fill(colors);
			var newSprite =
				new Sprite(
					new Material(ContentLoader.Load<Shader>(Shader.Position2DColorUv), customImage),
					new Rectangle(0.5f, 0.5f, 0.5f, 0.5f));
			var newRect = Rectangle.FromCenter(0.5f, 0.5f, 0.2f, 0.2f);
			newSprite.DrawArea = newRect;
			scene.Add(newSprite);
			bool freeName = false;
			int numberOfNames = 0;
			while (freeName == false)
				if (UIImagesInList.Contains("NewSprite" + numberOfNames))
					numberOfNames++;
				else
					freeName = true;
			UIImagesInList.Add("NewSprite" + numberOfNames);
		}

		public string SelectedSpriteNameInList
		{
			get { return selectedSpriteNameInList; }
			set
			{
				selectedSpriteNameInList = value;
				SelectedSprite = (Sprite)scene.Controls[SpriteListIndex];
			}
		}

		private string selectedSpriteNameInList;
		public int SpriteListIndex { get; set; }
	}
}