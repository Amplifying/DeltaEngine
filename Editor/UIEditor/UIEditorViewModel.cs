using System.Collections.Generic;
using System.Collections.ObjectModel;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager;
using DeltaEngine.Editor.Core;
using DeltaEngine.Input;
using DeltaEngine.Rendering2D.Shapes;
using DeltaEngine.Rendering2D.Sprites;
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
			Messenger.Default.Register<string>(this, "SaveUI", SaveUI);
			Messenger.Default.Register<string>(this, "ChangeMaterial", ChangeMaterial);
			Messenger.Default.Register<int>(this, "ChangeRenderLayer", ChangeRenderLayer);
			Messenger.Default.Register<bool>(this, "SetDraggingImage", SetDraggingImage);
			Messenger.Default.Register<object>(this, "ChangeGrid", ChangeGrid);
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
				if (Is2DMaterial(material))
					MaterialList.Add(material);
		}

		private static bool Is2DMaterial(string materialContentName)
		{
			var material = ContentLoader.Load<Material>(materialContentName);
			return material.Shader.Name.Contains("2D");
		}

		private void SetMouseCommands()
		{
			new Command(SetSelectedImage).Add(new MouseButtonTrigger());
			new Command(
				position =>
					ControlProcessor.MoveImage(position, SelectedSprite, isDragging, IsSnappingToGrid)).Add(
						new MousePositionTrigger(MouseButton.Left, State.Pressed));
			new Command(position => ControlProcessor.lastMousePosition = position).Add(
				new MouseButtonTrigger(MouseButton.Middle));
			new Command(position => AddImage(position)).Add(new MouseButtonTrigger(MouseButton.Left,
				State.Releasing));
		}

		private bool isDragging;
		public bool IsSnappingToGrid { get; set; }

		private void SetSelectedImage(Vector2D mousePosition)
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
				}
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

		public void AddImage(Vector2D position)
		{
			if (!isDragging)
				return;
			var sprite = AddNewImageToList(position);
			SelectedSprite = sprite;
			isDragging = false;
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
			var metaDataCreator = new ContentMetaDataCreator();
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

		private void SetDraggingImage(bool draggingImage)
		{
			isDragging = draggingImage;
		}

		private void ChangeGrid(object grid)
		{
			if (grid.ToString().Contains("10"))
				SetGridWidthAndHeight(10);
			else if (grid.ToString().Contains("16"))
				SetGridWidthAndHeight(16);
			else if (grid.ToString().Contains("20"))
				SetGridWidthAndHeight(20);
			else if (grid.ToString().Contains("24"))
				SetGridWidthAndHeight(24);
			else if (grid.ToString().Contains("50"))
				SetGridWidthAndHeight(50);
			else
				SetGridWidthAndHeight(0);
		}

		private void SetGridWidthAndHeight(int widthAndHeight)
		{
			GridWidth = widthAndHeight;
			GridHeight = widthAndHeight;
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

		private void DrawGrid()
		{
			foreach (Line2D line2D in linesInGridList)
				line2D.IsActive = false;
			linesInGridList.Clear();
			if (GridWidth == 0 || GridHeight == 0 || !isDrawingGrid)
				return;
			for (int i = 0; i < GridWidth; i++)
				linesInGridList.Add(new Line2D(new Vector2D((i * (1.0f / GridWidth)), 0),
					new Vector2D((i * (1.0f / GridWidth)), 1), Color.Red));
			for (int j = 0; j < GridHeight; j++)
				linesInGridList.Add(new Line2D(new Vector2D(0, (j * (1.0f / GridHeight))),
					new Vector2D(1, (j * (1.0f / GridHeight))), Color.Red));
		}

		private readonly List<Line2D> linesInGridList = new List<Line2D>();

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
		public bool IsShowingGrid
		{
			get { return isDrawingGrid; }
			set
			{
				isDrawingGrid = value;
				DrawGrid();
			}
		}

		private bool isDrawingGrid;

		private void DeleteUIElement()
		{
			UIImagesInList.RemoveAt(SpriteListIndex);
			scene.Controls.Remove(SelectedSprite);
			SelectedSprite.IsActive = false;
		}

		public Sprite AddNewImageToList(Vector2D position)
		{
			var newSprite = CreateNewImage(position);
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

		private static Sprite CreateNewImage(Vector2D position)
		{
			var customImage = ContentLoader.Create<Image>(new ImageCreationData(new Size(8, 8)));
			var colors = new Color[8 * 8];
			for (int i = 0; i < 8 * 8; i++)
				colors[i] = Color.Purple;
			customImage.Fill(colors);
			return
				new Sprite(
					new Material(ContentLoader.Load<Shader>(Shader.Position2DColorUv), customImage), position);
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