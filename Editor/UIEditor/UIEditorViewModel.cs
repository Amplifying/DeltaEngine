using System.Collections.Generic;
using System.Collections.ObjectModel;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.Core;
using DeltaEngine.Input;
using DeltaEngine.Rendering.Sprites;
using DeltaEngine.Scenes;
using DeltaEngine.ScreenSpaces;
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
			new Command(position => LastMousePosition = position).Add(
				new MouseButtonTrigger(MouseButton.Middle));
			new Command(ScaleImage).Add(new MousePositionTrigger(MouseButton.Middle, State.Pressed));
		}

		private void SetSelectedImage(Point mousePosition)
		{
			LastMousePosition = mousePosition;
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
			var relativePosition = mousePosition - LastMousePosition;
			LastMousePosition = mousePosition;
			SelectedSprite.Center += relativePosition;
		}

		private void ScaleImage(Point mousePosition)
		{
			var relativePosition = mousePosition - LastMousePosition;
			LastMousePosition = mousePosition;
			SelectedSprite.Size =
				new Size(SelectedSprite.Size.Width + (SelectedSprite.Size.Width * relativePosition.Y),
					SelectedSprite.Size.Height + (SelectedSprite.Size.Height * relativePosition.Y));
		}

		private Point LastMousePosition = Point.Unused;
		public Sprite SelectedSprite { get; set; }

		public void AddImage(string image)
		{
			if (string.IsNullOrEmpty(SelectedImageInList))
				return;

			AddNewImageToList(image);
			RaisePropertyChanged("UIImages");
			RaisePropertyChanged("ImageInGridList");
		}

		public string SelectedImageInList { get; set; }

		public void AddNewImageToList(string newImage)
		{
			var sprite = new Sprite(new Material(Shader.Position2DUv, SelectedImageInList),
				Rectangle.One);
			var size = sprite.Material.DiffuseMap.PixelSize;
			var newRect = new Rectangle(0, 0, size.Width, size.Height);
			newRect = ScreenSpace.Current.FromPixelSpace(newRect);
			sprite.DrawArea = newRect;
			scene.Add(sprite);
			UIImagesInList.Add(SelectedImageInList);
		}

		public string SelectedSpriteNameInList
		{
			get { return selectedSpriteNameInList; }
			set { selectedSpriteNameInList = value;
			SelectedSprite = (Sprite)scene.Controls[SpriteListIndex];
			}
		}

		private string selectedSpriteNameInList;
		public int SpriteListIndex { get; set; }

		public void RemoveSprite(string obj)
		{
			if (string.IsNullOrEmpty(SelectedSpriteNameInList) || SelectedSprite == null)
				return;
			UIImagesInList.Remove(SelectedSpriteNameInList);
			scene.Controls.Remove(SelectedSprite);
			SelectedSprite.IsActive = false;
		}
	}
}