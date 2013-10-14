using System.Collections.Generic;
using System.Collections.ObjectModel;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager;
using DeltaEngine.Editor.Core;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.ImageAnimationEditor
{
	public class AnimationEditorViewModel : ViewModelBase
	{
		public AnimationEditorViewModel(Service service)
		{
			this.service = service;
			metaDataCreator = new ContentMetaDataCreator();
			LoadedImageList = new ObservableCollection<string>();
			ImageList = new ObservableCollection<string>();
			AnimationList = new ObservableCollection<string>();
			Duration = 1;
			SubImageSize = new Size(500, 500);
			LoadImagesFromProject();
			LoadAnimationsFromProject();
			SetMessengers();
			service.ContentUpdated += SendSuccessMessageToLogger;
		}

		private readonly Service service;
		private readonly ContentMetaDataCreator metaDataCreator;
		public ObservableCollection<string> LoadedImageList { get; set; }
		public ObservableCollection<string> ImageList { get; set; }
		public ObservableCollection<string> AnimationList { get; set; }

		private void LoadImagesFromProject()
		{
			LoadedImageList.Clear();
			var foundContent = service.GetAllContentNamesByType(ContentType.Image);
			foreach (string content in foundContent)
				LoadedImageList.Add(content);
			RaisePropertyChanged("LoadedImageList");
		}

		private void LoadAnimationsFromProject()
		{
			AnimationList.Clear();
			var foundImageAnimtaion = service.GetAllContentNamesByType(ContentType.ImageAnimation);
			var foundSpriteSheetAnimtaion =
				service.GetAllContentNamesByType(ContentType.SpriteSheetAnimation);
			foreach (var imageAnimation in foundImageAnimtaion)
				AnimationList.Add(imageAnimation);
			foreach (var imageAnimation in foundSpriteSheetAnimtaion)
				AnimationList.Add(imageAnimation);
			RaisePropertyChanged("AnimtaionList");
		}

		private void SetMessengers()
		{
			Messenger.Default.Register<string>(this, "DeletingImage", DeleteImage);
			Messenger.Default.Register<int>(this, "MoveImageUp", MoveImageUp);
			Messenger.Default.Register<int>(this, "MoveImageDown", MoveImageDown);
			Messenger.Default.Register<string>(this, "SaveAnimation", SaveAnimation);
		}

		public void DeleteImage(string image)
		{
			ImageList.Remove(image);
			CreateNewAnimation();
			RaisePropertyChanged("ImageList");
		}

		public void MoveImageUp(int imageIndex)
		{
			if (imageIndex == 0)
				return;
			var imageToMove = ImageList[imageIndex];
			ImageList.RemoveAt(imageIndex);
			ImageList.Insert(imageIndex - 1, imageToMove);
			SelectedIndex = imageIndex - 1;
			CreateNewAnimation();
			RaisePropertyChanged("ImageList");
		}

		public void SaveAnimation(string obj)
		{
			if (ImageList.Count == 0 || string.IsNullOrEmpty(AnimationName))
				return;
			ContentMetaData contentMetaData = SaveImageAnimationOrSpriteSheetAnimation();
			if (ContentLoader.Exists(AnimationName))
				ReplaceOldContent();
			service.UploadContent(contentMetaData);
			service.ContentUpdated += SendSuccessMessageToLogger;
		}

		private ContentMetaData SaveImageAnimationOrSpriteSheetAnimation()
		{
			ContentMetaData contentMetaData;
			if (ImageList.Count == 1)
				contentMetaData = metaDataCreator.CreateMetaDataFromSpriteSheetAnimation(AnimationName,
					spriteSheetAnimation);
			else
				contentMetaData = metaDataCreator.CreateMetaDataFromImageAnimation(AnimationName, animation);
			return contentMetaData;
		}

		private void ReplaceOldContent()
		{
			service.DeleteContent(AnimationName);
			ContentLoader.RemoveResource(AnimationName);
		}

		private void SendSuccessMessageToLogger(ContentType type, string name)
		{
			Logger.Info("The saving of the animation called " + AnimationName + " was a succes.");
			service.ContentUpdated -= SendSuccessMessageToLogger;
		}

		public int SelectedIndex
		{
			get { return selectedIndex; }
			set
			{
				selectedIndex = value;
				RaisePropertyChanged("SelectedIndex");
			}
		}

		private int selectedIndex;

		public void MoveImageDown(int imageIndex)
		{
			if (imageIndex == ImageList.Count - 1)
				return;
			var imageToMove = ImageList[imageIndex];
			ImageList.RemoveAt(imageIndex);
			ImageList.Insert(imageIndex + 1, imageToMove);
			SelectedIndex = imageIndex + 1;
			CreateNewAnimation();
			RaisePropertyChanged("ImageList");
		}

		public string SelectedImageToAdd
		{
			set
			{
				ImageList.Add(value);
				CreateNewAnimation();
				RaisePropertyChanged("ImageList");
			}
		}

		private void CreateNewAnimation()
		{
			EntitiesRunner.Current.Clear();
			if (ImageList.Count == 1)
				ShowSpritesheetAnimation();
			if (ImageList.Count > 1)
				ShowMultipleImageAnimation();
		}

		public void ShowSpritesheetAnimation()
		{
			spriteSheetAnimation =
				new SpriteSheetAnimation(
					new SpriteSheetAnimationCreationData(ContentLoader.Load<Image>(ImageList[0]), Duration,
						SubImageSize));
			var material = new Material(Shader.Position2DUV, "") { SpriteSheet = spriteSheetAnimation };
			new Sprite(material, new Rectangle(0.25f, 0.25f, 0.5f, 0.5f));
		}

		private SpriteSheetAnimation spriteSheetAnimation;

		public float Duration
		{
			get { return duration; }
			set
			{
				duration = value;
				CreateNewAnimation();
			}
		}

		private float duration;

		public Size SubImageSize
		{
			get { return subImageSize; }
			set
			{
				subImageSize = value;
				CreateNewAnimation();
			}
		}

		private Size subImageSize;

		private void ShowMultipleImageAnimation()
		{
			var imagelist = new List<Image>();
			foreach (var image in ImageList)
				imagelist.Add(ContentLoader.Load<Image>(image));
			animation = new ImageAnimation(imagelist.ToArray(), Duration);
			new Sprite(new Material(Shader.Position2DUV, "") { Animation = animation },
				new Rectangle(0.25f, 0.25f, 0.5f, 0.5f));
		}

		private ImageAnimation animation;

		public string AnimationName
		{
			get { return animationName; }
			set
			{
				animationName = value;
				if (ContentLoader.Exists(animationName, ContentType.ImageAnimation) ||
					ContentLoader.Exists(animationName, ContentType.SpriteSheetAnimation))
					CreateAnimtaionFromFile();
			}
		}

		private string animationName;

		public void CreateAnimtaionFromFile()
		{
			EntitiesRunner.Current.Clear();
			ImageList.Clear();
			var material = new Material(Shader.Position2DUV, animationName);
			new Sprite(material, new Rectangle(0.25f, 0.25f, 0.5f, 0.5f));
			if (material.Animation != null)
				foreach (var image in material.Animation.Frames)
					ImageList.Add(image.Name);
			else
				ImageList.Add(material.SpriteSheet.Image.Name);
		}

		public void RefreshData()
		{
			LoadImagesFromProject();
		}
	}
}