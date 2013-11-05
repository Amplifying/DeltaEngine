using System.Collections.Generic;
using System.Collections.ObjectModel;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager;
using DeltaEngine.Editor.Core;
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
			LoadImagesFromProject();
			LoadAnimationsFromProject();
			SetMessengers();
			CreateDefaultMaterial2D();
			AnimationName = "MyAnimation";
			CheckIfCanSave();
			CanSaveAnimation = false;
		}

		private readonly Service service;
		private readonly ContentMetaDataCreator metaDataCreator;
		public ObservableCollection<string> LoadedImageList { get; set; }
		public ObservableCollection<string> ImageList { get; set; }
		public ObservableCollection<string> AnimationList { get; set; }

		private void LoadImagesFromProject()
		{
			LoadedImageList = new ObservableCollection<string>();
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
			Messenger.Default.Register<string>(this, "AddImage", AddImage);
		}

		private void CreateDefaultMaterial2D()
		{
			var imageData = new ImageCreationData(new Size(8.0f, 8.0f));
			var image = ContentLoader.Create<Image>(imageData);
			var colors = new Color[8 * 8];
			for (int i = 0; i < 8; i++)
				for (int j = 0; j < 8; j++)
					if ((i + j) % 2 == 0)
						colors[i * 8 + j] = Color.LightGray;
					else
						colors[i * 8 + j] = Color.DarkGray;
			image.Fill(colors);
			spriteSheetAnimation =
				new SpriteSheetAnimation(new SpriteSheetAnimationCreationData(image, 1, new Size(2, 2)));
			var material = new Material(Shader.Position2DUV, "") { SpriteSheet = spriteSheetAnimation };
			renderExample = new Sprite(material, new Rectangle(0.25f, 0.25f, 0.5f, 0.5f));
		}

		public void DeleteImage(string image)
		{
			ImageList.Remove(image);
			CreateNewAnimation();
			CheckIfCanSave();
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

		public void AddImage(string obj)
		{
			if (selectedImage == null)
				return;
			IsDisplayingAnimation = true;
			ImageList.Add(selectedImage);
			CreateNewAnimation();
			CheckIfCanSave();
			RaisePropertyChanged("ImageList");
		}

		//ncrunch: no coverage start
		public void SendSuccessMessageToLogger(ContentType type, string name)
		{
			Logger.Info("The saving of the animation called " + AnimationName + " was a success.");
			service.ContentUpdated -= SendSuccessMessageToLogger;
		}

		//ncrunch: no coverage end

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
				selectedImage = value;
				if (IsDisplayingImage)
					CreateDisplayedImage();
			}
		}

		private string selectedImage;

		private void CreateNewAnimation()
		{
			if (renderExample != null)
				renderExample.IsActive = false;
			if (ImageList.Count == 1)
				ShowSpritesheetAnimation();
			else if (ImageList.Count > 1)
				ShowMultipleImageAnimation();
		}

		public void ShowSpritesheetAnimation()
		{
			var image = ContentLoader.Load<Image>(ImageList[0]);
			if (SubImageSize.Width == 0 || SubImageSize.Height == 0)
			{
				subImageSize = image.PixelSize;
				RaisePropertyChanged("SubImageSize");
			}
			spriteSheetAnimation =
				new SpriteSheetAnimation(new SpriteSheetAnimationCreationData(image, Duration, SubImageSize));
			var material = new Material(Shader.Position2DUV, "") { SpriteSheet = spriteSheetAnimation };
			renderExample = new Sprite(material,
				Rectangle.FromCenter(0.5f, 0.5f, 0.5f * SubImageSize.AspectRatio, 0.5f));
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
				if (value.Width < 0 || value.Height < 0)
					return;
				subImageSize = value;
				if (value.Width > spriteSheetAnimation.Image.PixelSize.Width)
					subImageSize.Width = spriteSheetAnimation.Image.PixelSize.Width;
				if (value.Height > spriteSheetAnimation.Image.PixelSize.Height)
					subImageSize.Height = spriteSheetAnimation.Image.PixelSize.Height;
				CreateNewAnimation();
				RaisePropertyChanged("SubImageSize");
			}
		}

		private Size subImageSize;

		private void ShowMultipleImageAnimation()
		{
			var imagelist = new List<Image>();
			foreach (var image in ImageList)
				imagelist.Add(ContentLoader.Load<Image>(image));
			animation = new ImageAnimation(imagelist.ToArray(), Duration);
			renderExample = new Sprite(new Material(Shader.Position2DUV, "") { Animation = animation },
				Rectangle.FromCenter(0.5f, 0.5f, 0.5f * imagelist[0].PixelSize.AspectRatio, 0.5f));
		}

		private ImageAnimation animation;
		private Entity2D renderExample;

		public string AnimationName
		{
			get { return animationName; }
			set
			{
				animationName = value;
				CheckIfCanSave();
				if (ContentLoader.Exists(animationName, ContentType.ImageAnimation) ||
					ContentLoader.Exists(animationName, ContentType.SpriteSheetAnimation))
					CreateAnimationFromFile();
			}
		}

		private string animationName;

		public void CreateAnimationFromFile()
		{
			if (renderExample != null)
				renderExample.IsActive = false;
			ImageList.Clear();
			var material = new Material(Shader.Position2DUV, animationName);
			renderExample = new Sprite(material,
				Rectangle.FromCenter(0.5f, 0.5f, 0.5f * material.MaterialRenderSize.AspectRatio, 0.5f));
			if (material.Animation != null)
				foreach (var image in material.Animation.Frames)
					ImageList.Add(image.Name);
			else
				ImageList.Add(material.SpriteSheet.Image.Name);
		}

		public bool IsDisplayingImage
		{
			get { return isDisplayingImage; }
			set
			{
				isDisplayingImage = value;
				ChangeDisplay();
				RaisePropertyChanged("IsDisplayingImage");
			}
		}

		private bool isDisplayingImage;

		private void ChangeDisplay()
		{
			if (isDisplayingImage)
				CreateDisplayedImage();
			else
				CreateNewAnimation();
		}

		private void CreateDisplayedImage()
		{
			if (renderExample != null)
				renderExample.IsActive = false;
			renderExample = new Sprite(new Material(Shader.Position2DColorUV, selectedImage),
				Rectangle.HalfCentered);
		}

		public bool IsDisplayingAnimation
		{
			get { return !isDisplayingImage; }
			set
			{
				isDisplayingImage = !value;
				ChangeDisplay();
				RaisePropertyChanged("IsDisplayingAnimation");
			}
		}

		internal void ResetOnProjectChange()
		{
			RefreshOnContentChange();
		}

		internal void RefreshOnContentChange()
		{
			LoadedImageList.Clear();
			var foundContent = service.GetAllContentNamesByType(ContentType.Image);
			foreach (string content in foundContent)
				LoadedImageList.Add(content);
			RaisePropertyChanged("LoadedImageList");
		}

		public void ActivateAnimation()
		{
			renderExample.IsActive = true;
			service.Viewport.CenterViewOn(renderExample.Center);
			service.Viewport.ZoomViewTo(1.0f);
		}

		public bool CanSaveAnimation
		{
			get { return canSaveAnimation; }
			set
			{
				canSaveAnimation = value;
				RaisePropertyChanged("CanSaveAnimation");
			}
		}

		private bool canSaveAnimation;

		private void CheckIfCanSave()
		{
			if (string.IsNullOrEmpty(AnimationName) || ImageList.Count == 0)
				CanSaveAnimation = false;
			else
				CanSaveAnimation = true;
		}
	}
}