using System.Collections.Generic;
using System.Collections.ObjectModel;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager;
using DeltaEngine.Editor.Core;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering2D.Shapes;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.UserInterfaces.Controls;
using DeltaEngine.ScreenSpaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.UIEditor
{
	public class UIEditorViewModel : ViewModelBase
	{
		public UIEditorViewModel(Service service)
		{
			EntitiesRunner.Current.Clear();
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
			Messenger.Default.Register<bool>(this, "SetDraggingButton", SetDraggingButton);
			Messenger.Default.Register<bool>(this, "SetDraggingLabel", SetDraggingLabel);
			Messenger.Default.Register<object>(this, "ChangeGrid", ChangeGrid);
			Messenger.Default.Register<string>(this, "DeleteSelectedControl", DeleteSelectedControl);
			new Command(DeleteUIElement).Add(new KeyTrigger(Key.Delete));
		}

		public readonly Service service;
		public ObservableCollection<string> ContentImageListList { get; private set; }
		public ObservableCollection<string> UIImagesInList { get; private set; }
		public ObservableCollection<string> MaterialList { get; private set; }
		public readonly ControlProcessor ControlProcessor;
		public readonly Scene scene;

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
			Material material;
			try
			{
				material = ContentLoader.Load<Material>(materialContentName);
			}
				//ncrunch: no coverage start
			catch
			{
				return false;
			} //ncrunch: no coverage end
			return (!(material.Shader as ShaderWithFormat).Format.Is3D);
		}

		private void SetMouseCommands()
		{
			new Command(SetSelectedEntity2D).Add(new MouseButtonTrigger());
			new Command(
				position =>
					ControlProcessor.MoveImage(position, SelectedEntity2D, isDragging, IsSnappingToGrid)).Add(
						new MousePositionTrigger(MouseButton.Left, State.Pressed));
			new Command(position => ControlProcessor.lastMousePosition = position).Add(
				new MouseButtonTrigger(MouseButton.Middle));
			new Command(position => SetCommandsForReleasing(position)).Add(
				new MouseButtonTrigger(MouseButton.Left, State.Releasing));
		}

		private bool isDraggingButton;
		private bool isDragging;
		public bool IsSnappingToGrid { get; set; }

		private void SetSelectedEntity2D(Vector2D mousePosition)
		{
			if (SelectedEntity2D != null)
				if (SelectedEntity2D.GetType() == typeof(InteractiveButton) &&
					SelectedEntity2D.DrawArea.Contains(mousePosition))
					isClicking = true;
			ControlProcessor.lastMousePosition = mousePosition;
			foreach (Sprite sprite in scene.Controls)
				if (sprite.DrawArea.Contains(mousePosition))
				{
					Rectangle drawArea;
					if (sprite.GetType() == typeof(InteractiveButton))
						drawArea = new Rectangle(sprite.DrawArea.TopLeft, ((InteractiveButton)sprite).BaseSize);
					else
						drawArea = sprite.DrawArea;
					SpriteListIndex = scene.Controls.IndexOf(sprite);
					if (SpriteListIndex < 0)
						return; //ncrunch: no coverage 
					SelectedSpriteNameInList = sprite.Material.DiffuseMap.Name;
					SelectedEntity2D = sprite;
					ControlProcessor.UpdateOutLines(SelectedEntity2D);
					SetWidthAndHeight(drawArea);
				}
			RaisePropertyChanged("SelectedSpriteNameInList");
			RaisePropertyChanged("SpriteListIndex");
			RaisePropertyChanged("Entity2DWidth");
			RaisePropertyChanged("Entity2DHeight");
		}

		private void SetWidthAndHeight(Rectangle rect)
		{
			Entity2DWidth = rect.Width;
			Entity2DHeight = rect.Height;
		}

		private bool isClicking;
		public Entity2D SelectedEntity2D { get; set; }

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
			SpriteListIndex = scene.Controls.IndexOf(SelectedEntity2D);
			if (SpriteListIndex < 0)
				return; //ncrunch: no coverage 
			UIImagesInList[SpriteListIndex] = ControlName;
			selectedSpriteNameInList = ControlName;
			SelectedEntity2D.Get<Material>().MetaData.Name = ControlName;
			RaisePropertyChanged("UIImagesInList");
			RaisePropertyChanged("ControlName");
		}

		private void SetCommandsForReleasing(Vector2D position)
		{
			AddImage(position);
			AddButton(position);
			AddLabel(position);
			isClicking = false;
			if (SelectedEntity2D != null)
				ControlProcessor.UpdateOutLines(SelectedEntity2D);
		}

		public void AddImage(Vector2D position)
		{
			if (!isDraggingImage)
				return;
			var sprite = AddNewImageToList(position);
			SelectedEntity2D = sprite;
			Entity2DWidth = sprite.DrawArea.Width;
			Entity2DHeight = sprite.DrawArea.Height;
			SelectedEntity2D = sprite;
			isDraggingImage = false;
			isDragging = false;
			RaisePropertyChanged("UIImagesInList");
			RaisePropertyChanged("ImageInGridList");
		}

		private bool isDraggingImage;

		public Sprite AddNewImageToList(Vector2D position)
		{
			var costumImage = CreateDefaultImage();
			var newSprite =
				new Sprite(
					new Material(ContentLoader.Load<Shader>(Shader.Position2DColorUV), costumImage),
					new Rectangle(new Vector2D(0.45f, 0.45f), new Size(0.05f)));
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

		private static Image CreateDefaultImage()
		{
			var creationData = new ImageCreationData(new Size(8));
			var colors = new Color[8 * 8];
			for (int i = 0; i < 8; i++)
				for (int j = 0; j < 8; j++)
					if ((i + j) % 2 == 0)
						colors[i * 8 + j] = Color.LightGray;
					else
						colors[i * 8 + j] = Color.DarkGray;
			var costumImage = ContentLoader.Create<Image>(creationData);
			costumImage.Fill(colors);
			return costumImage;
		}

		public void AddButton(Vector2D position)
		{
			if (!isDraggingButton)
				return;
			var button = AddNewButtonToList(position);
			SelectedEntity2D = button;
			ContentText = "Default Button";
			Entity2DWidth = button.DrawArea.Width;
			Entity2DHeight = button.DrawArea.Height;
			SelectedEntity2D = button;
			isDraggingButton = false;
			isDragging = false;
			RaisePropertyChanged("UIImagesInList");
			RaisePropertyChanged("ImageInGridList");
		}

		private Entity2D AddNewButtonToList(Vector2D position)
		{
			var newButton = new InteractiveButton(new Rectangle(position, new Size(0.2f, 0.1f)),
				"Default Button");
			scene.Add(newButton);
			bool freeName = false;
			int numberOfNames = 0;
			while (freeName == false)
				if (UIImagesInList.Contains("NewButton" + numberOfNames))
					numberOfNames++;
				else
					freeName = true;
			UIImagesInList.Add("NewButton" + numberOfNames);
			if (UIImagesInList[0] == null)
				UIImagesInList[0] = "NewButton" + numberOfNames;
			newButton.theme.Button.Material.MetaData.Name = "NewButton" + numberOfNames;
			newButton.theme.ButtonDisabled.Material.MetaData.Name = "NewButton" + numberOfNames;
			newButton.theme.ButtonMouseover.Material.MetaData.Name = "NewButton" + numberOfNames;
			newButton.theme.ButtonPressed.Material.MetaData.Name = "NewButton" + numberOfNames;
			return newButton;
		}

		private void AddLabel(Vector2D position)
		{
			if (!isDraggingLabel)
				return;
			var label = AddNewLabelToList(position);
			SelectedEntity2D = label;
			ContentText = "Default Label";
			Entity2DWidth = label.DrawArea.Width;
			Entity2DHeight = label.DrawArea.Height;
			SelectedEntity2D = label;
			isDraggingLabel = false;
			isDragging = false;
			RaisePropertyChanged("UIImagesInList");
			RaisePropertyChanged("ImageInGridList");
		}

		private bool isDraggingLabel;

		private Entity2D AddNewLabelToList(Vector2D position)
		{
			var newLabel = new Label(new Rectangle(position, new Size(0.2f, 0.1f)), "DefaultLabel");
			scene.Add(newLabel);
			bool freeName = false;
			int numberOfNames = 0;
			while (freeName == false)
				if (UIImagesInList.Contains("NewLabel" + numberOfNames))
					numberOfNames++;
				else
					freeName = true;
			UIImagesInList.Add("NewLabel" + numberOfNames);
			if (UIImagesInList[0] == null)
				UIImagesInList[0] = "NewLabel" + numberOfNames;
			newLabel.Material.MetaData.Name = "NewLabel" + numberOfNames;
			return newLabel;
		}

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

		public void ChangeMaterial(string newMaterialName)
		{
			if (SelectedEntity2D == null)
				return;
			SelectedEntity2D.Set(ContentLoader.Load<Material>(newMaterialName));
			SelectedEntity2D.DrawArea = new Rectangle(SelectedEntity2D.DrawArea.TopLeft,
				ScreenSpace.Current.FromPixelSpace(SelectedEntity2D.Get<Material>().DiffuseMap.PixelSize));
			Entity2DWidth = SelectedEntity2D.DrawArea.Width;
			Entity2DHeight = SelectedEntity2D.DrawArea.Height;
			ControlProcessor.UpdateOutLines(SelectedEntity2D);
		}

		public void ChangeRenderLayer(int changeValue)
		{
			ControlLayer += changeValue;
			RaisePropertyChanged("ControlLayer");
		}

		public void SetDraggingImage(bool draggingImage)
		{
			isDraggingImage = draggingImage;
			isDragging = draggingImage;
		}

		public void SetDraggingButton(bool draggingButton)
		{
			isDraggingButton = draggingButton;
			isDragging = draggingButton;
		}

		public void SetDraggingLabel(bool draggingLabel)
		{
			isDraggingLabel = draggingLabel;
			isDragging = draggingLabel;
		}

		public void ChangeGrid(object grid)
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

		public void DeleteSelectedControl(string obj)
		{
			if (SelectedEntity2D == null)
				return;
			SpriteListIndex = scene.Controls.IndexOf(SelectedEntity2D);
			if (SpriteListIndex < 0)
				return; //ncrunch: no coverage 
			UIImagesInList.RemoveAt(SpriteListIndex);
			scene.Remove(SelectedEntity2D);
			SelectedEntity2D = null;
			SpriteListIndex = -1;
			ControlProcessor.UpdateOutLines(SelectedEntity2D);
		}

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
			if (SpriteListIndex < 0)
				return; //ncrunch: no coverage
			UIImagesInList.RemoveAt(SpriteListIndex);
			scene.Controls.Remove(SelectedEntity2D);
			SelectedEntity2D.IsActive = false;
		}

		public string SelectedSpriteNameInList
		{
			get { return selectedSpriteNameInList; }
			set
			{
				if (string.IsNullOrEmpty(value) || SpriteListIndex < 0)
					return;
				selectedSpriteNameInList = value;
				SelectedEntity2D = scene.Controls[SpriteListIndex];
				ControlProcessor.UpdateOutLines(SelectedEntity2D);
				ControlName = SelectedEntity2D.Get<Material>().MetaData.Name;
				ControlLayer = SelectedEntity2D.RenderLayer;
				Entity2DWidth = SelectedEntity2D.DrawArea.Width;
				Entity2DHeight = SelectedEntity2D.DrawArea.Height;
				if (SelectedEntity2D.GetType() == typeof(InteractiveButton))
				{
					var button = (InteractiveButton)SelectedEntity2D;
					ContentText = button.Text;
				}
				else
					ContentText = "";
				RaisePropertyChanged("ControlLayer");
				RaisePropertyChanged("Entity2DWidth");
				RaisePropertyChanged("Entity2DHeight");
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
				if (SelectedEntity2D == null)
					return;
				SelectedEntity2D.RenderLayer = controlLayer;
			}
		}

		private int controlLayer;

		public float Entity2DWidth
		{
			get { return entity2DWidth; }
			set
			{
				if (isClicking || SelectedEntity2D == null)
					return;
				entity2DWidth = value;
				var rect = SelectedEntity2D.DrawArea;
				rect.Width = entity2DWidth;
				SelectedEntity2D.DrawArea = rect;
				if (SelectedEntity2D.GetType() == typeof(InteractiveButton))
					ChangeButton((InteractiveButton)SelectedEntity2D);
				ControlProcessor.UpdateOutLines(SelectedEntity2D);
				RaisePropertyChanged("Entity2DWidth");
			}
		}

		private float entity2DWidth;

		public float Entity2DHeight
		{
			get { return entity2DHeight; }
			set
			{
				if (isClicking || SelectedEntity2D == null)
					return;
				entity2DHeight = value;
				var rect = SelectedEntity2D.DrawArea;
				rect.Height = entity2DHeight;
				SelectedEntity2D.DrawArea = rect;
				if (SelectedEntity2D.GetType() == typeof(InteractiveButton))
					ChangeButton((InteractiveButton)SelectedEntity2D);
				ControlProcessor.UpdateOutLines(SelectedEntity2D);
				RaisePropertyChanged("Entity2DHeight");
			}
		}

		private float entity2DHeight;

		private void ChangeButton(InteractiveButton button)
		{
			button.BaseSize = new Size(Entity2DWidth, Entity2DHeight);
			button.Text = ContentText;
		}

		public string ContentText
		{
			get { return contentText; }
			set
			{
				contentText = value;
				if (SelectedEntity2D.GetType() == typeof(InteractiveButton))
					ChangeButton((InteractiveButton)SelectedEntity2D);
				RaisePropertyChanged("ContentText");
			}
		}

		private string contentText;
	}
}