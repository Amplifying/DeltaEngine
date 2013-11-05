using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager;
using DeltaEngine.Editor.Core;
using DeltaEngine.Entities;
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
			this.service = service;
			Messenger.Default.Send("ClearScene", "ClearScene");
			Messenger.Default.Send("UIEditor", "SetSelectedEditorPlugin");
			uiEditorScene = new UIEditorScene();
			uiEditorScene.ControlProcessor = new ControlProcessor(this);
			uiControl = new UIControl();
			controlAdder = new ControlAdder();
			controlChanger = new ControlChanger();
			Adder = new ControlAdder();
			Scene = new Scene();
			InitializeVariables();
			FillContentImageList();
			FillMaterialList();
			FillSceneNames();
			FillListOfAvailableFonts();
			SetMouseCommands("");
			SetMessengers();
			CreateCenteredControl("Button");
			UIName = "MyUIName";
			CheckIfCanSaveScene();
			new Command(() => DeleteSelectedControl("")).Add(new KeyTrigger(Key.Delete));
		}

		public UIEditorScene uiEditorScene;
		public ControlAdder Adder { get; private set; }
		public readonly ControlAdder controlAdder;
		public readonly ControlChanger controlChanger;
		public readonly UIControl uiControl;

		private void InitializeVariables()
		{
			ContentImageListList = new ObservableCollection<string>();
			UIImagesInList = new ObservableCollection<string>();
			MaterialList = new ObservableCollection<string>();
			SceneNames = new ObservableCollection<string>();
			ResolutionList = new ObservableCollection<string>();
			AvailableFontsInProject = new ObservableCollection<string>();
			uiEditorScene.UIWidth = 1024;
			uiEditorScene.UIHeight = 768;
			FillResolutionListWithDefaultResolutions();
			uiEditorScene.CreateGrtidOutline();
			uiEditorScene.UpdateGridOutline();
		}

		public ObservableCollection<string> ResolutionList
		{
			get { return uiEditorScene.ResolutionList; }
			set { uiEditorScene.ResolutionList = value; }
		}

		private void FillResolutionListWithDefaultResolutions()
		{
			ResolutionList.Add("10 x 10");
			ResolutionList.Add("16 x 16");
			ResolutionList.Add("20 x 20");
			ResolutionList.Add("24 x 24");
			ResolutionList.Add("50 x 50");
			SelectedResolution = "20 x 20";
			IsShowingGrid = true;
		}

		public ObservableCollection<string> ContentImageListList
		{
			get { return uiEditorScene.ContentImageListList; }
			set { uiEditorScene.ContentImageListList = value; }
		}

		public ObservableCollection<string> UIImagesInList
		{
			get { return uiEditorScene.UIImagesInList; }
			set { uiEditorScene.UIImagesInList = value; }
		}

		public ObservableCollection<string> MaterialList
		{
			get { return uiEditorScene.MaterialList; }
			set { uiEditorScene.MaterialList = value; }
		}

		public ObservableCollection<string> SceneNames
		{
			get { return uiEditorScene.SceneNames; }
			set { uiEditorScene.SceneNames = value; }
		}

		public readonly Service service;

		private void FillContentImageList()
		{
			uiEditorScene.ContentImageListList.Clear();
			var imageList = service.GetAllContentNamesByType(ContentType.Image);
			foreach (var image in imageList)
				uiEditorScene.ContentImageListList.Add(image);
			RaisePropertyChanged("ContentImageListList");
		}

		private void FillMaterialList()
		{
			uiEditorScene.MaterialList.Clear();
			var materialList = service.GetAllContentNamesByType(ContentType.Material);
			foreach (var material in materialList.Where(material => TryAddMaterial(material)))
				uiEditorScene.MaterialList.Add(material);
			RaisePropertyChanged("MaterialList");
		}

		private static bool TryAddMaterial(string material)
		{
			try
			{
				ContentLoader.Load<Material>(material);
				return true;
			}
			catch
			{
				return false;
			}
		}

		private void FillSceneNames()
		{
			uiEditorScene.SceneNames.Clear();
			foreach (var newScene in service.GetAllContentNamesByType(ContentType.Scene))
				uiEditorScene.SceneNames.Add(newScene);
			RaisePropertyChanged("SceneNames");
		}

		private void FillListOfAvailableFonts()
		{
			AvailableFontsInProject.Clear();
			var fontsInProject = service.GetAllContentNamesByType(ContentType.Font);
			foreach (var fontName in fontsInProject)
				AvailableFontsInProject.Add(fontName);
			if (AvailableFontsInProject.Count > 0)
				SelectedFontName = AvailableFontsInProject[0];
			RaisePropertyChanged("AvailableFontsInProject");
			RaisePropertyChanged("SelectedFontName");
		}

		private void SetMouseCommands(string obj)
		{
			var mouseClick = new MouseButtonTrigger();
			mouseClick.AddTag("ViewControl");
			new Command(FindEntity2DOnPosition).Add(mouseClick).AddTag("ViewControl");
			var moveMouse = new MousePositionTrigger(MouseButton.Left, State.Pressed);
			moveMouse.AddTag("ViewControl");
			new Command(
				position =>
					uiEditorScene.ControlProcessor.MoveImage(position, SelectedEntity2D, Adder.IsDragging,
						uiEditorScene.IsSnappingToGrid, uiEditorScene)).Add(moveMouse).AddTag("ViewControl");
			var middleMouseClick = new MouseButtonTrigger(MouseButton.Middle);
			middleMouseClick.AddTag("ViewControl");
			new Command(position => uiEditorScene.ControlProcessor.lastMousePosition = position).Add(
				middleMouseClick).AddTag("ViewControl");
			var releaseMiddleMouse = new MouseButtonTrigger(MouseButton.Left, State.Releasing);
			releaseMiddleMouse.AddTag("ViewControl");
			new Command(position => SetCommandsForReleasing(position)).Add(releaseMiddleMouse).AddTag(
				"ViewControl");
		}

		private void DeleteSelectedContentFromWpf(string control)
		{
			var index = uiEditorScene.UIImagesInList.IndexOf(control);
			if (index < 0)
				return;
			uiEditorScene.UIImagesInList.RemoveAt(index);
			Scene.Controls[index].IsActive = false;
			Scene.Controls.RemoveAt(index);
			SelectedEntity2D = null;
			uiEditorScene.ControlProcessor.UpdateOutLines(SelectedEntity2D);
		}

		public void ClearScene(string obj)
		{
			foreach (var control in Scene.Controls)
				control.IsActive = false;
			Scene.Controls.Clear();
			UIImagesInList.Clear();
			SelectedEntity2D = null;
			foreach (var entity in EntitiesRunner.Current.GetEntitiesOfType<DrawableEntity>())
				entity.IsActive = false;
			SetMouseCommands("");
		}

		public Entity2D SelectedEntity2D
		{
			get { return uiEditorScene.SelectedEntity2D; }
			set { uiEditorScene.SelectedEntity2D = value; }
		}

		private void SetMessengers()
		{
			Messenger.Default.Register<string>(this, "SaveUI", SaveUI);
			Messenger.Default.Register<string>(this, "ChangeMaterial", ChangeMaterial);
			Messenger.Default.Register<string>(this, "ChangeHoveredMaterial", ChangeHoveredMaterial);
			Messenger.Default.Register<string>(this, "ChangePressedMaterial", ChangePressedMaterial);
			Messenger.Default.Register<string>(this, "ChangeDisabledMaterial", ChangeDisabledMaterial);
			Messenger.Default.Register<int>(this, "ChangeRenderLayer", ChangeRenderLayer);
			Messenger.Default.Register<bool>(this, "SetDraggingImage", Adder.SetDraggingImage);
			Messenger.Default.Register<bool>(this, "SetDraggingButton", Adder.SetDraggingButton);
			Messenger.Default.Register<bool>(this, "SetDraggingLabel", Adder.SetDraggingLabel);
			Messenger.Default.Register<bool>(this, "SetDraggingSlider", Adder.SetDraggingSlider);
			Messenger.Default.Register<object>(this, "ChangeGrid", ChangeGrid);
			Messenger.Default.Register<string>(this, "AddNewResolution", AddNewResolution);
			Messenger.Default.Register<string>(this, "SetSelectedNameFromHierachy",
				SetSelectedNameFromHierachy);
			Messenger.Default.Register<int>(this, "SetSelectedIndexFromHierachy",
				SetSelectedIndexFromHierachy);
			Messenger.Default.Register<string>(this, "SetCenteredControl", CreateCenteredControl);
			Messenger.Default.Register<string>(this, "SetMouseCommands", SetMouseCommands);
			Messenger.Default.Register<string>(this, "DeleteSelectedContentFromWpf",
				DeleteSelectedContentFromWpf);
			Messenger.Default.Register<string>(this, "ClearScene", ClearScene);
		}

		public void FindEntity2DOnPosition(Vector2D mousePosition)
		{
			if (SelectedEntity2D != null)
				if (SelectedEntity2D.GetType() == typeof(Button) &&
					SelectedEntity2D.DrawArea.Contains(mousePosition))
					uiControl.isClicking = true;
			uiEditorScene.ControlProcessor.lastMousePosition = mousePosition;
			bool hasSelectedControl = false;
			SelectedEntity2D = null;
			foreach (Control control in Scene.Controls)
				if (control.DrawArea.Contains(mousePosition))
				{
					if (SelectedEntity2D != null && control.RenderLayer < SelectedEntity2D.RenderLayer)
						continue;
					hasSelectedControl = true;
					if (SetEntity2D(control))
						return;
				}
			if (!hasSelectedControl)
				controlAdder.IsDragging = true;
			RaiseAllProperties();
		}

		private void RaiseAllProperties()
		{
			RaisePropertyChanged("SelectedSpriteNameInList");
			RaisePropertyChanged("SpriteListIndex");
			RaisePropertyChanged("Entity2DWidth");
			RaisePropertyChanged("Entity2DHeight");
			RaisePropertyChanged("ControlName");
			RaisePropertyChanged("ControlLayer");
			RaisePropertyChanged("ContentText");
			RaisePropertyChanged("UIImagesInList");
			RaisePropertyChanged("ImageInGridList");
			RaisePropertyChanged("ResolutionList");
		}

		public Scene Scene
		{
			get { return uiEditorScene.Scene; }
			set { uiEditorScene.Scene = value; }
		}

		private bool SetEntity2D(Control control)
		{
			Rectangle drawArea = control.GetType() == typeof(Button)
				? new Rectangle(control.DrawArea.TopLeft, (control).Size) : control.DrawArea;
			ControlListIndex = Scene.Controls.IndexOf(control);
			if (ControlListIndex < 0)
				return true; //ncrunch: no coverage 
			SelectedControlNameInList = control.Get<string>();
			SelectedEntity2D = control;
			if (SelectedEntity2D.Get<Material>() != null)
				if (MaterialList.Contains(SelectedEntity2D.Get<string>()))
					SelectedMaterial = SelectedEntity2D.Get<string>();
				else
					SetMaterialsToNull();
			Messenger.Default.Send(SelectedControlNameInList, "SetSelectedName");
			Messenger.Default.Send(ControlListIndex, "SetSelectedIndex");
			uiEditorScene.ControlProcessor.UpdateOutLines(SelectedEntity2D);
			SetWidthAndHeight(drawArea);
			ControlLayer = control.RenderLayer;
			return false;
		}

		private void SetMaterialsToNull()
		{
			SelectedMaterial = "";
			Messenger.Default.Send("SetMaterialToNull", "SetMaterialToNull");
			Messenger.Default.Send(SelectedEntity2D.GetType().ToString(), "SetHoveredMaterialToNull");
			Messenger.Default.Send(SelectedEntity2D.GetType().ToString(), "SetPressedMaterialToNull");
			Messenger.Default.Send(SelectedEntity2D.GetType().ToString(), "SetDisabledMaterialToNull");
			Messenger.Default.Send("SetHorizontalAllignmentToNull", "SetHorizontalAllignmentToNull");
			Messenger.Default.Send("SetVerticalAllignmentToNull", "SetVerticalAllignmentToNull");
		}

		private void SetWidthAndHeight(Rectangle rect)
		{
			Entity2DWidth = rect.Width;
			Entity2DHeight = rect.Height;
			topMargin = rect.Top;
			bottomMargin = rect.Bottom;
			leftMargin = rect.Left;
			rightMargin = rect.Right;
			RaisePropertyChanged("RightMargin");
			RaisePropertyChanged("LeftMargin");
			RaisePropertyChanged("BottomMargin");
			RaisePropertyChanged("TopMargin");
		}

		public string ControlName
		{
			get { return uiControl.ControlName; }
			set { controlChanger.ChangeControlName(value, uiControl, uiEditorScene); }
		}

		private void SetCommandsForReleasing(Vector2D position)
		{
			Adder.AddImage(position, uiControl, uiEditorScene);
			Adder.AddButton(position, uiControl, uiEditorScene);
			Adder.AddLabel(position, uiControl, uiEditorScene);
			Adder.AddSlider(position, uiControl, uiEditorScene);
			controlAdder.IsDragging = false;
			uiControl.isClicking = false;
			if (SelectedEntity2D != null)
				uiEditorScene.ControlProcessor.UpdateOutLines(SelectedEntity2D);
		}

		public void SaveUI(string obj)
		{
			if (Scene.Controls.Count == 0 || string.IsNullOrEmpty(UIName))
				return; //ncrunch: no coverage 
			var fileNameAndBytes = new Dictionary<string, byte[]>();
			var bytes = BinaryDataExtensions.ToByteArrayWithTypeInformation(Scene);
			fileNameAndBytes.Add(UIName + ".deltascene", bytes);
			var metaDataCreator = new ContentMetaDataCreator();
			ContentMetaData contentMetaData = metaDataCreator.CreateMetaDataFromUI(UIName, bytes);
			service.ContentUpdated += SendSuccessMessageToLogger;
			service.UploadContent(contentMetaData, fileNameAndBytes);
		}

		//ncrunch: no coverage start
		private void SendSuccessMessageToLogger(ContentType type, string content)
		{
			Logger.Info("The saving of the scene called " + UIName + " was a succes.");
			service.ContentUpdated -= SendSuccessMessageToLogger;
		}

		//ncrunch: no coverage end

		public string UIName
		{
			get { return uiEditorScene.UIName; }
			set
			{
				uiEditorScene.UIName = value;
				CheckIfCanSaveScene();
				if (!ContentLoader.Exists(uiEditorScene.UIName, ContentType.Scene))
					return;
				foreach (var entity in EntitiesRunner.Current.GetEntitiesOfType<DrawableEntity>())
					entity.IsActive = false;
				Messenger.Default.Send("ClearScene", "ClearScene");
				var scene = ContentLoader.Load<Scene>(uiEditorScene.UIName);
				Scene = new Scene();
				foreach (Control control in scene.Controls)
				{
					control.IsActive = true;
					UIImagesInList.Add(control.Get<string>());
					AddControlToScene(control);
					Messenger.Default.Send(control.Get<string>(), "AddToHierachyList");
					control.IsActive = false;
				}
				uiEditorScene.ControlProcessor.CreateNewLines();
				CheckIfCanSaveScene();
				SetMouseCommands("");
			}
		}

		private void AddControlToScene(Control control)
		{
			Control newControl = null;
			if (control.GetType() == typeof(Picture))
				newControl = new Picture(control.Get<Theme>(), control.Get<Material>(), control.DrawArea);
			else if (control.GetType() == typeof(Label))
			{
				newControl = new Label(control.Get<Theme>(), control.DrawArea, (control as Label).Text);
				newControl.Set(control.Get<Material>());
			}
			else if (control.GetType() == typeof(Button))
				newControl = new Button(control.Get<Theme>(), control.DrawArea, (control as Button).Text);
			else if (control.GetType() == typeof(Slider))
				newControl = new Slider(control.Get<Theme>(), control.DrawArea);
			newControl.Set(control.Get<string>());
			newControl.RenderLayer = control.RenderLayer;
			Scene.Add(newControl);
		}

		public void ChangeMaterial(string newMaterialName)
		{
			var material = ContentLoader.Load<Material>(newMaterialName);
			if (SelectedEntity2D == null)
				return;
			if (SelectedEntity2D.GetType() == typeof(Button))
				SelectedEntity2D.Get<Theme>().Button = ContentLoader.Load<Material>(newMaterialName);
			else if (SelectedEntity2D.GetType() == typeof(Slider))
			{
				SelectedEntity2D.Get<Theme>().Slider = ContentLoader.Load<Material>(newMaterialName);
				SelectedEntity2D.Get<Theme>().SliderDisabled = ContentLoader.Load<Material>(newMaterialName);
			}
			else if (SelectedEntity2D.GetType() == typeof(Label))
			{
				SelectedEntity2D.Set(ContentLoader.Load<Material>(newMaterialName));
				SelectedEntity2D.Get<Theme>().Label = ContentLoader.Load<Material>(newMaterialName);
				SelectedEntity2D.Set(ContentLoader.Load<Material>(newMaterialName).DefaultColor);
			}
			else
				SelectedEntity2D.Set(material);
			SetControlSize(SelectedEntity2D, ContentLoader.Load<Material>(newMaterialName));
			var rect = SelectedEntity2D.DrawArea;
			Entity2DWidth = rect.Width;
			Entity2DHeight = rect.Height;
			uiEditorScene.ControlProcessor.UpdateOutLines(SelectedEntity2D);
		}

		private void ChangeHoveredMaterial(string newMaterialName)
		{
			if (SelectedEntity2D == null)
				return;
			if (SelectedEntity2D.GetType() == typeof(Button))
			{
				var button = SelectedEntity2D as Button;
				button.Get<Theme>().ButtonMouseover = ContentLoader.Load<Material>(newMaterialName);
			}
			else if (SelectedEntity2D.GetType() == typeof(Slider))
			{
				var slider = SelectedEntity2D as Slider;
				slider.Get<Theme>().SliderPointerMouseover = ContentLoader.Load<Material>(newMaterialName);
			}
		}

		private void ChangePressedMaterial(string newMaterialName)
		{
			if (SelectedEntity2D == null)
				return;
			if (SelectedEntity2D.GetType() == typeof(Button))
			{
				var button = SelectedEntity2D as Button;
				button.Get<Theme>().ButtonPressed = ContentLoader.Load<Material>(newMaterialName);
			}
			else if (SelectedEntity2D.GetType() == typeof(Slider))
			{
				var slider = SelectedEntity2D as Slider;
				slider.Get<Theme>().SliderPointer = ContentLoader.Load<Material>(newMaterialName);
			}
		}

		private void ChangeDisabledMaterial(string newMaterialName)
		{
			if (SelectedEntity2D == null)
				return;
			if (SelectedEntity2D.GetType() == typeof(Button))
			{
				var button = SelectedEntity2D as Button;
				button.Get<Theme>().ButtonDisabled = ContentLoader.Load<Material>(newMaterialName);
			}
			else if (SelectedEntity2D.GetType() == typeof(Slider))
			{
				var slider = SelectedEntity2D as Slider;
				slider.Get<Theme>().SliderPointerDisabled = ContentLoader.Load<Material>(newMaterialName);
			}
		}

		public void ChangeRenderLayer(int changeValue)
		{
			ControlLayer += changeValue;
			RaisePropertyChanged("ControlLayer");
		}

		public void ChangeGrid(object grid)
		{
			var newGridWidthAndHeight = grid.ToString().Trim(new[] { ' ' });
			var gridwidthAndHeight = newGridWidthAndHeight.Split((new[] { '{', ',', '}', 'x' }));
			int width;
			int.TryParse(gridwidthAndHeight[0], out width);
			int height;
			int.TryParse(gridwidthAndHeight[1], out height);
			if (width <= 0 || height <= 0)
				return;
			SetGridWidthAndHeight(width, height);
		}

		private void SetGridWidthAndHeight(int width, int height)
		{
			GridWidth = width;
			GridHeight = height;
		}

		public int GridWidth
		{
			get { return uiEditorScene.GridWidth; }
			set
			{
				uiEditorScene.GridWidth = value;
				DrawGrid();
			}
		}

		private void DrawGrid()
		{
			if (UIWidth <= 0 || UIHeight <= 0)
				return;
			var sceneSize = ScreenSpace.Current.FromPixelSpace(new Size(UIWidth, UIHeight));
			var tileSize = ScreenSpace.Current.FromPixelSpace(new Size(GridWidth, GridHeight));
			var xOffset = 0.5f;
			var yOffset = 0.5f;
			var aspect = sceneSize.Width / sceneSize.Height;
			if (aspect > 1)
				yOffset = 1 / (2 * aspect);
			else if (aspect < 1)
				xOffset = aspect / 2;
			foreach (Line2D line2D in uiEditorScene.LinesInGridList)
				line2D.IsActive = false;
			uiEditorScene.LinesInGridList.Clear();
			if (GridWidth == 0 || GridHeight == 0 || !uiEditorScene.IsDrawingGrid)
				return;
			if (yOffset < xOffset)
			{
				for (int i = 0; i < UIWidth / GridWidth + 1; i++)
					uiEditorScene.LinesInGridList.Add(
						new Line2D(
							new Vector2D((0.5f - xOffset + i * (1 / (sceneSize.Width / tileSize.Width))),
								0.5f - yOffset),
							new Vector2D((0.5f - xOffset + i * (1 / (sceneSize.Width / tileSize.Width))),
								1 - (0.5f - yOffset)), Color.Red));
				for (int j = 0; j < UIHeight / GridHeight + 1; j++)
					uiEditorScene.LinesInGridList.Add(
						new Line2D(
							new Vector2D(0.5f - xOffset,
								(0.5f - yOffset + j * (1 / (sceneSize.Height / tileSize.Height)) / aspect)),
							new Vector2D(1 - (0.5f - xOffset),
								(0.5f - yOffset + j * (1 / (sceneSize.Height / tileSize.Height)) / aspect)), Color.Red));
			}
			else
			{
				for (int i = 0; i < UIWidth / GridWidth + 1; i++)
					uiEditorScene.LinesInGridList.Add(
						new Line2D(
							new Vector2D((0.5f - xOffset + i * (1 / (sceneSize.Width / tileSize.Width)) * aspect),
								0.5f - yOffset),
							new Vector2D((0.5f - xOffset + i * (1 / (sceneSize.Width / tileSize.Width)) * aspect),
								1 - (0.5f - yOffset)), Color.Red));
				for (int j = 0; j < UIHeight / GridHeight + 1; j++)
					uiEditorScene.LinesInGridList.Add(
						new Line2D(
							new Vector2D(0.5f - xOffset,
								(0.5f - yOffset + j * (1 / (sceneSize.Height / tileSize.Height)))),
							new Vector2D(1 - (0.5f - xOffset),
								(0.5f - yOffset + j * (1 / (sceneSize.Height / tileSize.Height)))), Color.Red));
			}
		}

		public int GridHeight
		{
			get { return uiEditorScene.GridHeight; }
			set
			{
				uiEditorScene.GridHeight = value;
				DrawGrid();
			}
		}

		public void DeleteSelectedControl(string obj)
		{
			if (SelectedEntity2D == null)
				return; //ncrunch: no coverage
			uiControl.Index = Scene.Controls.IndexOf(SelectedEntity2D);
			if (uiControl.Index < 0)
				return; //ncrunch: no coverage
			uiEditorScene.UIImagesInList.RemoveAt(uiControl.Index);
			Scene.Remove(SelectedEntity2D);
			SelectedEntity2D = null;
			uiControl.Index = -1;
			uiEditorScene.ControlProcessor.UpdateOutLines(SelectedEntity2D);
			Messenger.Default.Send("", "DeleteSelectedContent");
		}

		private void AddNewResolution(string obj)
		{
			if (ResolutionList.Contains(NewGridWidth.ToString() + " x " + NewGridHeight.ToString()))
			{
				SelectedResolution = NewGridWidth.ToString() + " x " + NewGridHeight.ToString();
				return;
			}
			if (ResolutionList.Count > 9)
				for (int i = 0; i < 10; i++)
					if (i == 9)
						ResolutionList.RemoveAt(i);
					else
						ResolutionList[i] = ResolutionList[i + 1];
			RaiseAllProperties();
			ResolutionList.Add(NewGridWidth.ToString() + " x " + NewGridHeight.ToString());
			SelectedResolution = NewGridWidth.ToString() + " x " + NewGridHeight.ToString();
			RaisePropertyChanged("SelectedResolution");
		}

		private void SetSelectedNameFromHierachy(string newSelectedName)
		{
			SelectedControlNameInList = newSelectedName;
		}

		private void SetSelectedIndexFromHierachy(int newSelectedIndex)
		{
			ControlListIndex = newSelectedIndex;
		}

		private void CreateCenteredControl(string newControl)
		{
			Adder.IsDragging = true;
			if (newControl == "Image")
			{
				Adder.IsDraggingImage = true;
				Adder.AddImage(Vector2D.Half, uiControl, uiEditorScene);
			}
			else if (newControl == "Button")
			{
				Adder.IsDraggingButton = true;
				Adder.AddButton(Vector2D.Half, uiControl, uiEditorScene);
			}
			else if (newControl == "Label")
			{
				Adder.IsDraggingLabel = true;
				Adder.AddLabel(Vector2D.Half, uiControl, uiEditorScene);
			}
			else if (newControl == "Slider")
			{
				Adder.IsDraggingSlider = true;
				Adder.AddSlider(Vector2D.Half, uiControl, uiEditorScene);
			}
			uiControl.isClicking = false;
			if (SelectedEntity2D != null)
				uiEditorScene.ControlProcessor.UpdateOutLines(SelectedEntity2D);
		}

		public bool IsShowingGrid
		{
			get { return uiEditorScene.IsDrawingGrid; }
			set
			{
				uiEditorScene.IsDrawingGrid = value;
				DrawGrid();
			}
		}

		public string SelectedControlNameInList
		{
			get { return uiEditorScene.SelectedControlNameInList; }
			set
			{
				controlChanger.SetSelectedControlNameInList(value, uiControl, uiEditorScene);
				RaiseAllProperties();
			}
		}

		public int ControlLayer
		{
			get { return uiControl.controlLayer; }
			set { controlChanger.SetControlLayer(value, uiControl, uiEditorScene); }
		}

		public float Entity2DWidth
		{
			get { return uiControl.EntityWidth; }
			set { controlChanger.SetWidth(value, uiControl, uiEditorScene); }
		}

		public float Entity2DHeight
		{
			get { return uiControl.EntityHeight; }
			set { controlChanger.SetHeight(value, uiControl, uiEditorScene); }
		}

		public string ContentText
		{
			get { return uiControl.contentText; }
			set { controlChanger.SetContentText(value, uiControl, uiEditorScene); }
		}

		public int ControlListIndex
		{
			get { return uiControl.Index; }
			set { uiControl.Index = value; }
		}

		public bool IsSnappingToGrid
		{
			get { return uiEditorScene.IsSnappingToGrid; }
			set { uiEditorScene.IsSnappingToGrid = value; }
		}

		public int NewGridWidth { get; set; }
		public int NewGridHeight { get; set; }

		public string SelectedResolution
		{
			get { return uiEditorScene.SelectedResolution; }
			set
			{
				uiEditorScene.SelectedResolution = value;
				ChangeGrid(value);
			}
		}

		public string SelectedMaterial
		{
			get { return selectedMaterial; }
			set
			{
				selectedMaterial = value;
				RaisePropertyChanged("SelectedMaterial");
			}
		}

		private string selectedMaterial;

		public float BottomMargin
		{
			get { return bottomMargin; }
			set
			{
				bottomMargin = value;
				if (SelectedEntity2D != null)
					SelectedEntity2D.DrawArea = new Rectangle(SelectedEntity2D.DrawArea.Left,
						value - SelectedEntity2D.DrawArea.Height, SelectedEntity2D.DrawArea.Width,
						SelectedEntity2D.DrawArea.Height);
				SetWidthAndHeight(SelectedEntity2D.DrawArea);
			}
		}

		private float bottomMargin;

		public float TopMargin
		{
			get { return topMargin; }
			set
			{
				topMargin = value;
				if (SelectedEntity2D != null)
					SelectedEntity2D.DrawArea = new Rectangle(SelectedEntity2D.DrawArea.Left, value,
						SelectedEntity2D.DrawArea.Width, SelectedEntity2D.DrawArea.Height);
				SetWidthAndHeight(SelectedEntity2D.DrawArea);
			}
		}

		private float topMargin;

		public float LeftMargin
		{
			get { return leftMargin; }
			set
			{
				leftMargin = value;
				if (SelectedEntity2D != null)
					SelectedEntity2D.DrawArea = new Rectangle(value, SelectedEntity2D.DrawArea.Top,
						SelectedEntity2D.DrawArea.Width, SelectedEntity2D.DrawArea.Height);
				SetWidthAndHeight(SelectedEntity2D.DrawArea);
			}
		}

		private float leftMargin;

		public float RightMargin
		{
			get { return rightMargin; }
			set
			{
				rightMargin = value;
				if (SelectedEntity2D != null)
					SelectedEntity2D.DrawArea = new Rectangle(value - SelectedEntity2D.DrawArea.Width,
						SelectedEntity2D.DrawArea.Top, SelectedEntity2D.DrawArea.Width,
						SelectedEntity2D.DrawArea.Height);
				SetWidthAndHeight(SelectedEntity2D.DrawArea);
			}
		}

		private float rightMargin;

		public string HorizontalAllignment
		{
			get { return horizontalAllignment; }
			set
			{
				if (value == null)
					return;
				horizontalAllignment = value;
				if (value.Contains("Left"))
					LeftMargin = uiEditorScene.GridOutLine[0].StartPoint.X;
				if (value.Contains("Right"))
					RightMargin = uiEditorScene.GridOutLine[0].EndPoint.X;
				if (value.Contains("Center"))
					LeftMargin = 0.5f - SelectedEntity2D.DrawArea.Width / 2;
				RaisePropertyChanged("HorizontalAllignment");
			}
		}

		private string horizontalAllignment;

		public string VerticalAllignment
		{
			get { return verticalAllignment; }
			set
			{
				if (value == null)
					return;
				verticalAllignment = value;
				if (value.Contains("Top"))
					TopMargin = uiEditorScene.GridOutLine[0].StartPoint.Y;
				if (value.Contains("Bottom"))
					BottomMargin = uiEditorScene.GridOutLine[3].EndPoint.Y;
				if (value.Contains("Center"))
					TopMargin = 0.5f - SelectedEntity2D.DrawArea.Height / 2;
				RaisePropertyChanged("VerticalAllignment");
			}
		}

		private string verticalAllignment;

		public int UIWidth
		{
			get { return uiEditorScene.UIWidth; }
			set
			{
				uiEditorScene.UIWidth = value;
				foreach (var control in uiEditorScene.Scene.Controls)
					SetControlSize(control, control.Get<Material>());
				uiEditorScene.UpdateGridOutline();
				DrawGrid();
			}
		}

		public int UIHeight
		{
			get { return uiEditorScene.UIHeight; }
			set
			{
				uiEditorScene.UIHeight = value;
				foreach (var control in uiEditorScene.Scene.Controls)
					SetControlSize(control, control.Get<Material>());
				uiEditorScene.UpdateGridOutline();
				DrawGrid();
			}
		}

		private void SetControlSize(Entity2D control, Material material)
		{
			if (material.DiffuseMap.PixelSize.Width < 10 || material.DiffuseMap.PixelSize.Height < 10)
				return;
			if (UIWidth > UIHeight)
				control.DrawArea = new Rectangle(control.DrawArea.TopLeft,
					new Size(((material.DiffuseMap.PixelSize.Width / UIWidth)),
						((material.DiffuseMap.PixelSize.Height / UIWidth))));
			else
				control.DrawArea = new Rectangle(control.DrawArea.TopLeft,
					new Size(((material.DiffuseMap.PixelSize.Width / UIHeight)),
						((material.DiffuseMap.PixelSize.Height / UIHeight))));
		}

		internal void ResetOnProjectChange()
		{
			Messenger.Default.Send("ClearScene", "ClearScene");
			InitializeVariables();
			FillContentImageList();
			FillMaterialList();
			FillSceneNames();
		}

		internal void RefreshOnContentChange()
		{
			InitializeVariables();
			FillContentImageList();
			FillMaterialList();
			FillSceneNames();
		}

		public ObservableCollection<string> AvailableFontsInProject { get; set; }

		public string SelectedFontName { get; set; }

		public void ActivateHidenScene()
		{
			foreach (var control in uiEditorScene.Scene.Controls)
				control.IsActive = true;
			foreach (var line in uiEditorScene.ControlProcessor.OutLines)
				line.IsActive = true;
			foreach (var line in uiEditorScene.LinesInGridList)
				line.IsActive = true;
			foreach (var line in uiEditorScene.GridOutLine)
				line.IsActive = true;
			service.Viewport.CenterViewOn(Vector2D.Half);
			service.Viewport.ZoomViewTo(1.0f);
			Messenger.Default.Send("UIEditor", "SetSelectedEditorPlugin");
		}

		public bool CanSaveScene
		{
			get { return canSaveScene; }
			set
			{
				canSaveScene = value;
				RaisePropertyChanged("CanSaveScene");
			}
		}

		private bool canSaveScene;

		private void CheckIfCanSaveScene()
		{
			CanSaveScene = !string.IsNullOrEmpty(UIName);
		}
	}
}