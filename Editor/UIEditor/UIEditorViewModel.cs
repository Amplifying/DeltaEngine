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
			uiEditorScene = new UIEditorScene();
			uiEditorScene.ControlProcessor = new ControlProcessor(this);
			uiControl = new UIControl();
			controlAdder = new ControlAdder();
			controlChanger = new ControlChanger();
			Adder = new ControlAdder();
			InitializeVariables();
			FillContentImageList();
			FillMaterialList();
			FillSceneNames();
			SetMouseCommands("");
			SetMessengers();
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
			FillResolutionListWithDefaultResolutions();
			uiEditorScene.Scene = new Scene();
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
			var imageList = service.GetAllContentNamesByType(ContentType.Image);
			foreach (var image in imageList)
				uiEditorScene.ContentImageListList.Add(image);
		}

		private void FillMaterialList()
		{
			var materialList = service.GetAllContentNamesByType(ContentType.Material);
			foreach (var material in materialList)
				if (Is2DMaterial(material))
					uiEditorScene.MaterialList.Add(material);
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

		private void FillSceneNames()
		{
			foreach (var newScene in service.GetAllContentNamesByType(ContentType.Scene))
				uiEditorScene.SceneNames.Add(newScene);
		}

		private void SetMouseCommands(string obj)
		{
			new Command(FindEntity2DOnPosition).Add(new MouseButtonTrigger());
			new Command(
				position =>
					uiEditorScene.ControlProcessor.MoveImage(position, SelectedEntity2D, Adder.IsDragging,
						uiEditorScene.IsSnappingToGrid)).Add(new MousePositionTrigger(MouseButton.Left,
							State.Pressed));
			new Command(position => uiEditorScene.ControlProcessor.lastMousePosition = position).Add(
				new MouseButtonTrigger(MouseButton.Middle));
			new Command(position => SetCommandsForReleasing(position)).Add(
				new MouseButtonTrigger(MouseButton.Left, State.Releasing));
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
		}

		private void DeleteSelectedContentFromWpf(string control)
		{
			var index = uiEditorScene.UIImagesInList.IndexOf(control);
			if (index < 0)
				return;
			uiEditorScene.UIImagesInList.RemoveAt(index);
			Scene.Controls[index].IsActive = false; 
			Scene.Controls.RemoveAt(index);
			uiEditorScene.ControlProcessor.UpdateOutLines(SelectedEntity2D);
		}

		private void FindEntity2DOnPosition(Vector2D mousePosition)
		{
			if (SelectedEntity2D != null)
				if (SelectedEntity2D.GetType() == typeof(Button) &&
					SelectedEntity2D.DrawArea.Contains(mousePosition))
					uiControl.isClicking = true;
			uiEditorScene.ControlProcessor.lastMousePosition = mousePosition;
			foreach (Sprite sprite in Scene.Controls)
				if (sprite.DrawArea.Contains(mousePosition))
					if (SetEntity2D(sprite))
						return; //ncrunch: no coverage 
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

		private bool SetEntity2D(Sprite sprite)
		{
			Rectangle drawArea = sprite.GetType() == typeof(Button)
				? new Rectangle(sprite.DrawArea.TopLeft, (sprite).Size) : sprite.DrawArea;
			SpriteListIndex = Scene.Controls.IndexOf(sprite);
			if (SpriteListIndex < 0)
				return true; //ncrunch: no coverage 
			SelectedSpriteNameInList = sprite.Material.MetaData.Name;
			SelectedEntity2D = sprite;
			if (SelectedEntity2D.Get<Material>() != null)
				if (MaterialList.Contains(SelectedEntity2D.Get<Material>().MetaData.Name))
					SelectedMaterial = SelectedEntity2D.Get<Material>().MetaData.Name;
				else
				{
					SelectedMaterial = "";
					Messenger.Default.Send("SetMaterialToNull", "SetMaterialToNull");
				}
			Messenger.Default.Send(SelectedSpriteNameInList, "SetSelectedName");
			Messenger.Default.Send(SpriteListIndex, "SetSelectedIndex");
			uiEditorScene.ControlProcessor.UpdateOutLines(SelectedEntity2D);
			SetWidthAndHeight(drawArea);
			return false;
		}

		private void SetWidthAndHeight(Rectangle rect)
		{
			Entity2DWidth = rect.Width;
			Entity2DHeight = rect.Height;
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
			fileNameAndBytes.Add(UIName + ".deltaUI", bytes);
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
				if (!ContentLoader.Exists(uiEditorScene.UIName, ContentType.Scene))
					return;
				foreach (var entity in EntitiesRunner.Current.GetAllEntities())
					entity.IsActive = false;
				Scene = ContentLoader.Load<Scene>(uiEditorScene.UIName);
				foreach (var control in Scene.Controls)
					UIImagesInList.Add(control.Get<Material>().Name);
				SetMouseCommands("");
			}
		}

		public void ChangeMaterial(string newMaterialName)
		{
			if (SelectedEntity2D == null)
				return;
			if (SelectedEntity2D.GetType() == typeof(Button))
			{
				var button = SelectedEntity2D as Button;
				button.Get<Theme>().Button = ContentLoader.Load<Material>(newMaterialName);
				button.Get<Theme>().ButtonDisabled = ContentLoader.Load<Material>(newMaterialName);
				button.Get<Theme>().ButtonMouseover = ContentLoader.Load<Material>(newMaterialName);
				button.Get<Theme>().ButtonPressed = ContentLoader.Load<Material>(newMaterialName);
			}
			SelectedEntity2D.Set(ContentLoader.Load<Material>(newMaterialName));
			SelectedEntity2D.DrawArea = new Rectangle(SelectedEntity2D.DrawArea.TopLeft,
				ScreenSpace.Current.FromPixelSpace(SelectedEntity2D.Get<Material>().DiffuseMap.PixelSize));
			Entity2DWidth = SelectedEntity2D.DrawArea.Width;
			Entity2DHeight = SelectedEntity2D.DrawArea.Height;
			uiEditorScene.ControlProcessor.UpdateOutLines(SelectedEntity2D);
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
			foreach (Line2D line2D in uiEditorScene.linesInGridList)
				line2D.IsActive = false;
			uiEditorScene.linesInGridList.Clear();
			if (GridWidth == 0 || GridHeight == 0 || !uiEditorScene.IsDrawingGrid)
				return;
			for (int i = 0; i < GridWidth; i++)
				uiEditorScene.linesInGridList.Add(new Line2D(new Vector2D((i * (1.0f / GridWidth)), 0),
					new Vector2D((i * (1.0f / GridWidth)), 1), Color.Red));
			for (int j = 0; j < GridHeight; j++)
				uiEditorScene.linesInGridList.Add(new Line2D(new Vector2D(0, (j * (1.0f / GridHeight))),
					new Vector2D(1, (j * (1.0f / GridHeight))), Color.Red));
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
			if (ResolutionList.Count > 9)
				for (int i = 0; i < 10; i++)
					if (i == 9)
						ResolutionList.RemoveAt(i);
					else
						ResolutionList[i] = ResolutionList[i + 1];
			RaiseAllProperties();
			ResolutionList.Add(NewGridWidth.ToString() + " x " + NewGridHeight.ToString());
		}

		private void SetSelectedNameFromHierachy(string newSelectedName)
		{
			SelectedSpriteNameInList = newSelectedName;
		}

		private void SetSelectedIndexFromHierachy(int newSelectedIndex)
		{
			SpriteListIndex = newSelectedIndex;
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

		public string SelectedSpriteNameInList
		{
			get { return uiEditorScene.SelectedSpriteNameInList; }
			set
			{
				controlChanger.SetSelectedSpriteNameInList(value, uiControl, uiEditorScene);
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

		public int SpriteListIndex
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
	}
}