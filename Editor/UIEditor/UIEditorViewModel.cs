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
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.Controls;
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
			uiEditorScene = new UIEditorScene();
			uiEditorScene.ControlProcessor = new ControlProcessor(this);
			uiControl = new UIControl();
			controlAdder = new ControlAdder();
			controlChanger = new ControlChanger();
			Adder = new ControlAdder();
			Scene = new Scene();
			InitializeDefaults();
			InitializeGrid();
			FillContentImageList();
			FillMaterialList();
			FillSceneNames();
			FillListOfAvailableFonts();
			SetMouseCommands("");
			SetMessengers();
			CreateCenteredControl("Button");
			UIName = "MyScene";
			CheckIfCanSaveScene();
			new Command(() => DeleteSelectedControl("")).Add(new KeyTrigger(Key.Delete));
		}

		public UIEditorScene uiEditorScene;
		public ControlAdder Adder { get; private set; }
		public readonly ControlAdder controlAdder;
		public readonly ControlChanger controlChanger;
		public readonly UIControl uiControl;

		private void InitializeDefaults()
		{
			ContentImageListList = new ObservableCollection<string>();
			UIImagesInList = new ObservableCollection<string>();
			MaterialList = new ObservableCollection<string>();
			SceneNames = new ObservableCollection<string>();
			ResolutionList = new ObservableCollection<string>();
			uiEditorScene.AvailableFontsInProject = new ObservableCollection<string>();
			uiEditorScene.UIWidth = 1024;
			uiEditorScene.UIHeight = 768;
			NewGridWidth = 30;
			NewGridHeight = 30;
		}

		private void InitializeGrid()
		{
			FillResolutionListWithDefaultResolutions();
			uiEditorScene.CreateGritdOutline();
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
				//ncrunch: no coverage start
			catch
			{
				return false;
			} //ncrunch: no coverage end
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
			uiEditorScene.AvailableFontsInProject.Clear();
			var fontsInProject = service.GetAllContentNamesByType(ContentType.Font);
			foreach (var fontName in fontsInProject)
				uiEditorScene.AvailableFontsInProject.Add(fontName);
			if (uiEditorScene.AvailableFontsInProject.Count > 0)
				uiEditorScene.SelectedFontName = uiEditorScene.AvailableFontsInProject[0];
			RaisePropertyChanged("AvailableFontsInProject");
			RaisePropertyChanged("SelectedFontName");
		}

		private void SetMouseCommands(string obj)
		{
			var leftClickTrigger = new MouseButtonTrigger();
			leftClickTrigger.AddTag("temporary");
			var findEntityCommand = new Command(FindEntity2DOnPosition).Add(leftClickTrigger);
			findEntityCommand.AddTag("temporary");
			var moveMouse = new MousePositionTrigger(MouseButton.Left, State.Pressed);
			moveMouse.AddTag("temporary");
			var moveImageCommand =
				new Command(
					position =>
						uiEditorScene.ControlProcessor.MoveImage(position, SelectedEntity2D, Adder.IsDragging,
							uiEditorScene.IsSnappingToGrid, uiEditorScene)).Add(moveMouse);
			moveImageCommand.AddTag("temporary");
			var middleMouseClick = new MouseButtonTrigger(MouseButton.Middle);
			middleMouseClick.AddTag("temporary");
			var setLastPositionCommand =
				new Command(position => uiEditorScene.ControlProcessor.lastMousePosition = position).Add(
					middleMouseClick);
			setLastPositionCommand.AddTag("temporary");
			var releaseMiddleMouse = new MouseButtonTrigger(MouseButton.Left, State.Releasing);
			releaseMiddleMouse.AddTag("temporary");
			var setReleasingCommand =
				new Command(position => SetCommandsForReleasing(position)).Add(releaseMiddleMouse);
			setReleasingCommand.AddTag("temporary");
		}

		public void DeleteSelectedContentFromWpf(string control)
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
			set
			{
				uiEditorScene.SelectedEntity2D = value;
				EnableButtons = uiEditorScene.SelectedEntity2D != null;
				RaisePropertyChanged("EnableButtons");
			}
		}

		public bool EnableButtons
		{
			get { return uiEditorScene.EnableButtons; }
			set
			{
				uiEditorScene.EnableButtons = value;
				RaisePropertyChanged("EnableButtons");
			}
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
			for (int index = 0; index < Scene.Controls.Count; index++)
			{
				var control = (Control)Scene.Controls[index];
				if (control.DrawArea.Contains(mousePosition))
				{
					if (SelectedEntity2D != null && control.RenderLayer < SelectedEntity2D.RenderLayer)
						continue;
					hasSelectedControl = true;
					if (SetEntity2D(control))
						return;
				}
			}
			if (!hasSelectedControl)
				controlAdder.IsDragging = true;
			uiEditorScene.UpdateOutLine(SelectedEntity2D);
			UpdateMaterialsInViewPort();
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
			Rectangle drawArea;
			if (control.GetType() == typeof(Button))
			{
				drawArea = new Rectangle(control.DrawArea.TopLeft, (control).Size);
				IsInteractiveButton = false;
			}
			else if (control.GetType() == typeof(InteractiveButton))
			{
				drawArea = Rectangle.FromCenter(control.Center, ((InteractiveButton)control).BaseSize);
				if (IsInteractiveButton == false)
					IsInteractiveButton = true;
			}
			else
				drawArea = control.DrawArea;
			ControlListIndex = Scene.Controls.IndexOf(control);
			if (ControlListIndex < 0)
				return true; //ncrunch: no coverage 
			SelectedControlNameInList = control.GetTags()[0];
			SelectedEntity2D = control;
			SetMaterials();
			Messenger.Default.Send(SelectedControlNameInList, "SetSelectedName");
			Messenger.Default.Send(ControlListIndex, "SetSelectedIndex");
			uiEditorScene.ControlProcessor.UpdateOutLines(SelectedEntity2D);
			SetWidthAndHeight(drawArea);
			ControlLayer = control.RenderLayer;
			return false;
		}

		private void SetMaterials()
		{
			if (SelectedEntity2D.GetType() == typeof(Button))
			{
				SelectedMaterial = SelectedEntity2D.Get<Theme>().Button.Name;
				SelectedHoveredMaterial = SelectedEntity2D.Get<Theme>().ButtonMouseover.Name;
				SelectedPressedMaterial = SelectedEntity2D.Get<Theme>().ButtonPressed.Name;
				SelectedDisabledMaterial = SelectedEntity2D.Get<Theme>().ButtonDisabled.Name;
			}
			else if (SelectedEntity2D.GetType() == typeof(Slider))
			{
				SelectedMaterial = SelectedEntity2D.Get<Theme>().Slider.Name;
				SelectedHoveredMaterial = SelectedEntity2D.Get<Theme>().SliderPointerMouseover.Name;
				SelectedPressedMaterial = SelectedEntity2D.Get<Theme>().SliderPointer.Name;
				SelectedDisabledMaterial = SelectedEntity2D.Get<Theme>().SliderDisabled.Name;
			}
			else if (SelectedEntity2D.GetType() == typeof(Label))
			{
				SelectedMaterial = SelectedEntity2D.Get<Theme>().Label.Name;
				SelectedHoveredMaterial = null;
				SelectedPressedMaterial = null;
				SelectedDisabledMaterial = null;
			}
			else if (SelectedEntity2D.Get<Material>() != null)
			{
				SelectedMaterial = SelectedEntity2D.Get<Material>().ToString();
				SelectedHoveredMaterial = null;
				SelectedPressedMaterial = null;
				SelectedDisabledMaterial = null;
			}
			UpdateMaterialsInViewPort();
		}

		private void UpdateMaterialsInViewPort()
		{
			Messenger.Default.Send(SelectedMaterial, "SetMaterial");
			Messenger.Default.Send(SelectedHoveredMaterial, "SetHoveredMaterial");
			Messenger.Default.Send(SelectedPressedMaterial, "SetPressedMaterial");
			Messenger.Default.Send(SelectedDisabledMaterial, "SetDisabledMaterial");
			SetEnabledButtons(SelectedEntity2D != null ? SelectedEntity2D.GetType().ToString() : "");
			Messenger.Default.Send("SetHorizontalAllignmentToNull", "SetHorizontalAllignmentToNull");
			Messenger.Default.Send("SetHorizontalAllignmentToNull", "SetHorizontalAllignmentToNull");
			Messenger.Default.Send("SetVerticalAllignmentToNull", "SetVerticalAllignmentToNull");
		}

		private static void SetEnabledButtons(string type)
		{
			Messenger.Default.Send(type, "EnabledHoveredButton");
			Messenger.Default.Send(type, "EnabledPressedButton");
			Messenger.Default.Send(type, "EnabledDisableButton");
			Messenger.Default.Send(type, "EnableButtonChanger");
		}

		private void SetWidthAndHeight(Rectangle rect)
		{
			Entity2DWidth = rect.Width;
			Entity2DHeight = rect.Height;
			uiControl.TopMargin = rect.Top;
			uiControl.BottomMargin = rect.Bottom;
			uiControl.LeftMargin = rect.Left;
			uiControl.RightMargin = rect.Right;
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
			if (SelectedEntity2D == null)
				return;
			uiEditorScene.ControlProcessor.UpdateOutLines(SelectedEntity2D);
			SetEntity2D((Control)SelectedEntity2D);
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
			}
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
			uiControl.SetControlSize(SelectedEntity2D, ContentLoader.Load<Material>(newMaterialName),
				uiEditorScene);
			var rect = SelectedEntity2D.DrawArea;
			Entity2DWidth = rect.Width;
			Entity2DHeight = rect.Height;
			uiEditorScene.ControlProcessor.UpdateOutLines(SelectedEntity2D);
		}

		public void ChangeHoveredMaterial(string newMaterialName)
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

		public void ChangePressedMaterial(string newMaterialName)
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

		public void ChangeDisabledMaterial(string newMaterialName)
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
			if (ControlLayer < 0)
				ControlLayer = 0;
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
				uiEditorScene.DrawGrid();
			}
		}

		public int GridHeight
		{
			get { return uiEditorScene.GridHeight; }
			set
			{
				uiEditorScene.GridHeight = value;
				uiEditorScene.DrawGrid();
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

		public void AddNewResolution(string obj)
		{
			if (ResolutionList.Contains(NewGridWidth.ToString() + " x " + NewGridHeight.ToString()))
			{
				SelectedResolution = NewGridWidth.ToString() + " x " + NewGridHeight.ToString();
				return;
			}
			if (NewGridWidth <= 0 || NewGridHeight <= 0)
				return;
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

		public void SetSelectedNameFromHierachy(string newSelectedName)
		{
			SelectedControlNameInList = newSelectedName;
		}

		public void SetSelectedIndexFromHierachy(int newSelectedIndex)
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
			if (SelectedEntity2D == null)
				return;
			uiEditorScene.ControlProcessor.UpdateOutLines(SelectedEntity2D);
			SetEntity2D((Control)SelectedEntity2D);
		}

		public bool IsShowingGrid
		{
			get { return uiEditorScene.IsDrawingGrid; }
			set
			{
				uiEditorScene.IsDrawingGrid = value;
				uiEditorScene.DrawGrid();
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
			set
			{
				if (uiEditorScene.GridRenderLayer <= value)
				{
					uiEditorScene.GridRenderLayer = value + 1;
					uiEditorScene.UpdateRenderlayerGrid();
				}
				controlChanger.SetControlLayer(value, uiControl, uiEditorScene);
			}
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
				RaisePropertyChanged("SelectedResolution");
			}
		}

		public string SelectedMaterial
		{
			get { return uiControl.SelectedMaterial; }
			set
			{
				uiControl.SelectedMaterial = value;
				RaisePropertyChanged("SelectedMaterial");
			}
		}

		public string SelectedHoveredMaterial
		{
			get { return uiControl.SelectedHoveredMaterial; }
			set
			{
				uiControl.SelectedHoveredMaterial = value;
				RaisePropertyChanged("SelectedHoveredMaterial");
			}
		}

		public string SelectedPressedMaterial
		{
			get { return uiControl.SelectedPressedMaterial; }
			set
			{
				uiControl.SelectedPressedMaterial = value;
				RaisePropertyChanged("SelectedPressedMaterial");
			}
		}

		public string SelectedDisabledMaterial
		{
			get { return uiControl.SelectedDisabledMaterial; }
			set
			{
				uiControl.SelectedDisabledMaterial = value;
				RaisePropertyChanged("SelectedDisabledMaterial");
			}
		}

		public float BottomMargin
		{
			get { return uiControl.BottomMargin; }
			set
			{
				uiControl.BottomMargin = value;
				if (SelectedEntity2D != null)
					SelectedEntity2D.DrawArea = new Rectangle(SelectedEntity2D.DrawArea.Left,
						value - SelectedEntity2D.DrawArea.Height, SelectedEntity2D.DrawArea.Width,
						SelectedEntity2D.DrawArea.Height);
				SetWidthAndHeight(SelectedEntity2D.DrawArea);
			}
		}

		public float TopMargin
		{
			get { return uiControl.TopMargin; }
			set
			{
				uiControl.TopMargin = value;
				if (SelectedEntity2D != null)
					SelectedEntity2D.DrawArea = new Rectangle(SelectedEntity2D.DrawArea.Left, value,
						SelectedEntity2D.DrawArea.Width, SelectedEntity2D.DrawArea.Height);
				SetWidthAndHeight(SelectedEntity2D.DrawArea);
			}
		}

		public float LeftMargin
		{
			get { return uiControl.LeftMargin; }
			set
			{
				uiControl.LeftMargin = value;
				if (SelectedEntity2D != null)
					SelectedEntity2D.DrawArea = new Rectangle(value, SelectedEntity2D.DrawArea.Top,
						SelectedEntity2D.DrawArea.Width, SelectedEntity2D.DrawArea.Height);
				SetWidthAndHeight(SelectedEntity2D.DrawArea);
			}
		}

		public float RightMargin
		{
			get { return uiControl.RightMargin; }
			set
			{
				uiControl.RightMargin = value;
				if (SelectedEntity2D != null)
					SelectedEntity2D.DrawArea = new Rectangle(value - SelectedEntity2D.DrawArea.Width,
						SelectedEntity2D.DrawArea.Top, SelectedEntity2D.DrawArea.Width,
						SelectedEntity2D.DrawArea.Height);
				SetWidthAndHeight(SelectedEntity2D.DrawArea);
			}
		}

		public string HorizontalAllignment
		{
			get { return uiControl.HorizontalAllignment; }
			set
			{
				if (value == null || SelectedEntity2D == null)
					return;
				uiControl.HorizontalAllignment = value;
				if (value.Contains("Left"))
					LeftMargin = uiEditorScene.GridOutLine[0].StartPoint.X;
				if (value.Contains("Right"))
					RightMargin = uiEditorScene.GridOutLine[0].EndPoint.X;
				if (value.Contains("Center"))
					LeftMargin = 0.5f - SelectedEntity2D.DrawArea.Width / 2;
				RaisePropertyChanged("HorizontalAllignment");
			}
		}

		public string VerticalAllignment
		{
			get { return uiControl.VerticalAllignment; }
			set
			{
				if (value == null || SelectedEntity2D == null)
					return;
				uiControl.VerticalAllignment = value;
				if (value.Contains("Top"))
					TopMargin = uiEditorScene.GridOutLine[0].StartPoint.Y;
				if (value.Contains("Bottom"))
					BottomMargin = uiEditorScene.GridOutLine[3].EndPoint.Y;
				if (value.Contains("Center"))
					TopMargin = 0.5f - SelectedEntity2D.DrawArea.Height / 2;
				RaisePropertyChanged("VerticalAllignment");
			}
		}

		public int UIWidth
		{
			get { return uiEditorScene.UIWidth; }
			set
			{
				uiEditorScene.UIWidth = value;
				foreach (var control in uiEditorScene.Scene.Controls)
					uiControl.SetControlSize(control, control.Get<Material>(), uiEditorScene);
				uiEditorScene.UpdateGridOutline();
				uiEditorScene.DrawGrid();
				CheckIfCanSaveScene();
			}
		}

		public int UIHeight
		{
			get { return uiEditorScene.UIHeight; }
			set
			{
				uiEditorScene.UIHeight = value;
				foreach (var control in uiEditorScene.Scene.Controls)
					uiControl.SetControlSize(control, control.Get<Material>(), uiEditorScene);
				uiEditorScene.UpdateGridOutline();
				uiEditorScene.DrawGrid();
				CheckIfCanSaveScene();
			}
		}

		internal void ResetOnProjectChange()
		{
			Messenger.Default.Send("ClearScene", "ClearScene");
			InitializeGrid();
			FillContentImageList();
			FillMaterialList();
			FillSceneNames();
		}

		internal void RefreshOnContentChange()
		{
			FillContentImageList();
			FillMaterialList();
			FillSceneNames();
		}

		public void ActivateHiddenScene()
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
			SetMouseCommands("");
		}

		public bool CanSaveScene
		{
			get { return uiEditorScene.CanSaveScene; }
			set
			{
				uiEditorScene.CanSaveScene = value;
				RaisePropertyChanged("CanSaveScene");
			}
		}

		private void CheckIfCanSaveScene()
		{
			if (UIWidth == 0 || UIHeight == 0 || string.IsNullOrEmpty(UIName))
				CanSaveScene = false;
			else
				CanSaveScene = true;
		}

		public void LoadScene()
		{
			if (!ContentLoader.Exists(uiEditorScene.UIName, ContentType.Scene))
				return;
			foreach (var entity in EntitiesRunner.Current.GetEntitiesOfType<DrawableEntity>())
				entity.IsActive = false;
			Messenger.Default.Send("ClearScene", "ClearScene");
			try
			{
				var scene = ContentLoader.Load<Scene>(uiEditorScene.UIName);
				Scene = new Scene();
				foreach (Control control in scene.Controls)
				{
					control.IsActive = true;
					controlAdder.AddControlToScene(control, uiEditorScene);
					Messenger.Default.Send(control.GetTags()[0], "AddToHierachyList");
					control.IsActive = false;
					if (uiEditorScene.GridRenderLayer <= control.RenderLayer)
						uiEditorScene.GridRenderLayer = control.RenderLayer + 1;
				}
			}
			catch
			{
				return;
			}
			uiEditorScene.ControlProcessor.CreateNewLines();
			uiEditorScene.DrawGrid();
			CheckIfCanSaveScene();
			SetMouseCommands("");
		}

		public bool IsInteractiveButton
		{
			get { return isInteractiveButton; }
			set
			{
				isInteractiveButton = value;
				ChangeToInteractiveButton();
				RaisePropertyChanged("IsInteractiveButton");
			}
		}

		private bool isInteractiveButton;

		private void ChangeToInteractiveButton()
		{
			if (SelectedEntity2D == null || uiEditorScene == null ||
				(SelectedEntity2D.GetType() != typeof(Button) &&
					SelectedEntity2D.GetType() != typeof(InteractiveButton)))
				return;
			uiEditorScene.Scene.Remove(SelectedEntity2D);
			var renderLayer = SelectedEntity2D.RenderLayer;
			var controlName = SelectedEntity2D.GetTags()[0];
			if (isInteractiveButton)
				SelectedEntity2D = new InteractiveButton(SelectedEntity2D.Get<Theme>(),
					SelectedEntity2D.DrawArea, ((Button)SelectedEntity2D).Text);
			else
				SelectedEntity2D = new Button(SelectedEntity2D.Get<Theme>(), SelectedEntity2D.DrawArea,
					((Button)SelectedEntity2D).Text);
			SelectedEntity2D.RenderLayer = renderLayer;
			SelectedEntity2D.AddTag(controlName);
			uiEditorScene.Scene.Add(SelectedEntity2D);
		}
	}
}