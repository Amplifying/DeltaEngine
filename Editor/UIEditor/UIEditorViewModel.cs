using System.Collections.Generic;
using System.Collections.ObjectModel;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager;
using DeltaEngine.Editor.Core;
using DeltaEngine.Input;
using DeltaEngine.Rendering.Shapes;
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
			this.service = service;
			ContentImageListList = new ObservableCollection<string>();
			UIImagesInList = new ObservableCollection<string>();
			MaterialList = new ObservableCollection<string>();
			ControlProcessor = new ControlProcessor(this);
			scene = new Scene();
			FillContentImageList();
			FillMaterialList();
			SetMouseCommands();
			Messenger.Default.Register<string>(this, "AddImage", AddImage);
			Messenger.Default.Register<string>(this, "SaveUI", SaveUI);
			Messenger.Default.Register<string>(this, "ChangeMaterial", ChangeMaterial);
			Messenger.Default.Register<int>(this, "ChangeRenderLayer", ChangeRenderLayer);
			new Command(DeleteUIElement).Add(new KeyTrigger(Key.Delete));
		}

		private readonly Service service;
		public ObservableCollection<string> ContentImageListList { get; private set; }
		public ObservableCollection<string> UIImagesInList { get; private set; }
		public ObservableCollection<string> MaterialList { get; private set; }
		public readonly ControlProcessor ControlProcessor;
		public Scene scene;

		private void FillContentImageList()
		{
			var imageList = service.GetAllContentNamesByType(ContentType.Image);
			foreach (var image in imageList)
				ContentImageListList.Add(image);
		}

		private void FillMaterialList()
		{
			var materialList = service.GetAllContentNamesByType(ContentType.Material);
			foreach (var material in materialList)
				MaterialList.Add(material);
		}

		private void SetMouseCommands()
		{
			new Command(SetSelectedImage).Add(new MouseButtonTrigger());
			new Command(position => ControlProcessor.MoveImage(position, SelectedSprite)).Add(
				new MousePositionTrigger(MouseButton.Left, State.Pressed));
			new Command(position => ControlProcessor.lastMousePosition = position).Add(
				new MouseButtonTrigger(MouseButton.Middle));
		}

		private void SetSelectedImage(Point mousePosition)
		{
			ControlProcessor.lastMousePosition = mousePosition;
			//bool FoundSprite = false;
			foreach (Sprite sprite in scene.Controls)
				if (sprite.DrawArea.Contains(mousePosition))
				{
					SpriteListIndex = scene.Controls.IndexOf(sprite);
					SelectedSpriteNameInList = sprite.Material.DiffuseMap.Name;
					SelectedSprite = sprite;
					ControlProcessor.UpdateOutLines(SelectedSprite);
					//FoundSprite = true;
				}
			//if (!FoundSprite)
			//{
			//	SelectedSprite = null;
			//	SpriteListIndex = 0;
			//	SelectedSpriteNameInList = "";
			//	ControlProcessor.UpdateOutLines(SelectedSprite);
			//}
			RaisePropertyChanged("SelectedSpriteNameInList");
			RaisePropertyChanged("SpriteListIndex");
		}

		public Sprite SelectedSprite { get; set; }

		public string ControlName
		{
			get { return controlName; }
			set
			{
				controlName = value;
				ChangeControlName();
			}
		}

		private string controlName;

		private void ChangeControlName()
		{
			SpriteListIndex = scene.Controls.IndexOf(SelectedSprite);
			UIImagesInList[SpriteListIndex] = ControlName;
			selectedSpriteNameInList = ControlName;
			SelectedSprite.Material.MetaData.Name = ControlName;
			RaisePropertyChanged("UIImagesInList");
			RaisePropertyChanged("ControlName");
		}

		public void AddImage(string image)
		{
			var sprite = AddNewImageToList();
			SelectedSprite = sprite;
			RaisePropertyChanged("UIImagesInList");
			RaisePropertyChanged("ImageInGridList");
		}

		public string SelectedImageInList { get; set; }

		public void SaveUI(string obj)
		{
			if (scene.Controls.Count == 0 || string.IsNullOrEmpty(UIName))
				return;
			var fileNameAndBytes = new Dictionary<string, byte[]>();
			var bytes = BinaryDataExtensions.ToByteArrayWithTypeInformation(scene);
			fileNameAndBytes.Add(UIName + ".deltaUI", bytes);
			var metaDataCreator = new ContentMetaDataCreator(service);
			ContentMetaData contentMetaData = metaDataCreator.CreateMetaDataFromUI(UIName, bytes);
			service.UploadContent(contentMetaData, fileNameAndBytes);
			service.ContentUpdated += SendSuccessMessageToLogger;
		}

		private void SendSuccessMessageToLogger(ContentType type, string content)
		{
			Logger.Info("The saving of the animation called " + UIName + " was a succes.");
			service.ContentUpdated -= SendSuccessMessageToLogger;
		}

		public string UIName { get; set; }

		internal void ChangeMaterial(string newMaterialName)
		{
			SelectedSprite.Material = ContentLoader.Load<Material>(newMaterialName);
			SelectedSprite.DrawArea = new Rectangle(SelectedSprite.DrawArea.TopLeft,
				ScreenSpace.Current.FromPixelSpace(SelectedSprite.Material.DiffuseMap.PixelSize));
			ControlProcessor.UpdateOutLines(SelectedSprite);
		}

		private void ChangeRenderLayer(int changeValue)
		{
			ControlLayer += changeValue;
			RaisePropertyChanged("ControlLayer");
		}

		private void DeleteUIElement()
		{
			UIImagesInList.RemoveAt(SpriteListIndex);
			scene.Controls.Remove(SelectedSprite);
			SelectedSprite.IsActive = false;
		}

		public Sprite AddNewImageToList()
		{
			var newSprite = CreateNewImage();
			newSprite.DrawArea = Rectangle.FromCenter(0.5f, 0.5f, 0.2f, 0.2f);
			scene.Add(newSprite);
			bool freeName = false;
			int numberOfNames = 0;
			while (freeName == false)
				if (UIImagesInList.Contains("NewSprite" + numberOfNames))
					numberOfNames++;
				else
					freeName = true;
			UIImagesInList.Add("NewSprite" + numberOfNames);
			if (UIImagesInList[0] == null)
				UIImagesInList[0] = "NewSprite" + numberOfNames;
			newSprite.Material.MetaData.Name = "NewSprite" + numberOfNames;
			return newSprite;
		}

		private static Sprite CreateNewImage()
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
			newSprite.Material.MetaData = new ContentMetaData();
			return newSprite;
		}

		public string SelectedSpriteNameInList
		{
			get { return selectedSpriteNameInList; }
			set
			{
				if (value == "")
					return;
				selectedSpriteNameInList = value;
				SelectedSprite = (Sprite)scene.Controls[SpriteListIndex];
				ControlProcessor.UpdateOutLines(SelectedSprite);
				ControlName = SelectedSprite.Material.MetaData.Name;
				ControlLayer = SelectedSprite.RenderLayer;
				RaisePropertyChanged("ControlLayer");
			}
		}

		private string selectedSpriteNameInList;
		public int SpriteListIndex { get; set; }

		public bool IsShowingGrid
		{
			set
			{
				if (value)
					DrawGrid();
			}
		}

		private void DrawGrid()
		{
			if (GridWidth == 0 || GridHeight == 0)
				return;
			foreach (Line2D line2D in linesInGridList)
				line2D.IsActive = false;
			linesInGridList.Clear();
			for (int i = 0; i < GridWidth; i++)
				linesInGridList.Add(new Line2D(new Point((i * (1.0f / GridWidth)), 0),
					new Point((i * (1.0f / GridWidth)), 1), Color.Red));
			for (int j = 0; j < GridHeight; j++)
				linesInGridList.Add(new Line2D(new Point(0, (j * (1.0f / GridHeight))),
					new Point(1, (j * (1.0f / GridHeight))), Color.Red));
		}

		public int GridWidth
		{
			get { return gridWidth; }
			set
			{
				gridWidth = value;
				DrawGrid();
			}
		}

		private int gridWidth;

		public int GridHeight
		{
			get { return gridHeight; }
			set
			{
				gridHeight = value;
				DrawGrid();
			}
		}

		private int gridHeight;
		private readonly List<Line2D> linesInGridList = new List<Line2D>();

		public int ControlLayer
		{
			get { return controlLayer; }
			set
			{
				controlLayer = value;
				SelectedSprite.RenderLayer = controlLayer;
			}
		}

		private int controlLayer;
	}
}