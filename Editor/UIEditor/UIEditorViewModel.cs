using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions;
using System.Windows.Media.Imaging;
using DeltaEngine.Core.Xml;
using DeltaEngine.Datatypes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.UIEditor
{
	public class UIEditorViewModel : ViewModelBase
	{
		public UIEditorViewModel(IFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;
			imageProcessor = new ImageProcessor(this);
			CreateNewLists();
			string[] projects = fileSystem.Directory.GetDirectories(ContentPath);
			foreach (var project in projects)
				ProjectList.Add(Path.GetFileName(project));
			SetMessengerMethods();
			SetDefaultGridParameters();
			uiMetaDataSaverAndLoader = new UIMetaDataSaverAndLoader(fileSystem);
		}

		public IFileSystem fileSystem;
		private readonly ImageProcessor imageProcessor;
		public const string ContentPath = "Content";

		private void CreateNewLists()
		{
			IuImagesInList = new ObservableCollection<string>();
			ProjectList = new ObservableCollection<string>();
			UIImages = new ObservableCollection<UIImage>();
		}

		public ObservableCollection<string> IuImagesInList { get; private set; }
		public ObservableCollection<string> ProjectList { get; set; }
		public ObservableCollection<UIImage> UIImages { get; set; }

		private void SetMessengerMethods()
		{
			SetImageMovingMessengers();
			SetGridChangingMessengers();
			SetImageChangingMessengers();
			SetSaveAndLoadMessengers();
		}

		private void SetImageMovingMessengers()
		{
			Messenger.Default.Register<string>(this, "AddImage", AddImage);
			Messenger.Default.Register<Point>(this, "LeftMouseDown", LeftMouseDown);
			Messenger.Default.Register<Point>(this, "MouseMove", MouseMove);
			Messenger.Default.Register<Point>(this, "LeftMouseUp", LeftMouseUp);
		}

		public void AddImage(string imageLink)
		{
			if (string.IsNullOrEmpty(SelectedImageInList))
				return;

			imageProcessor.AddNewImageToList();
			RaisePropertyChanged("UIImages");
			RaisePropertyChanged("ImageInGridList");
		}

		public void LeftMouseDown(Point pos)
		{
			LeftMouseButtonDown = true;
			foreach (var image in UIImages)
				if (pos.X > image.X && pos.X < image.X + image.Width && pos.Y > image.Y &&
					pos.Y < image.Y + image.Height)
					imageProcessor.CalculateDistance(pos, image);
		}

		public bool LeftMouseButtonDown { get; set; }

		public void MouseMove(Point pos)
		{
			if (!LeftMouseButtonDown)
				return;

			imageProcessor.EditImagePosition(pos);
			RaisePropertyChanged("UIImages");
			RaisePropertyChanged("SelectedImage");
		}

		public UIImage SelectedImage { get; set; }

		public void LeftMouseUp(Point obj)
		{
			LeftMouseButtonDown = false;
		}

		private void SetGridChangingMessengers()
		{
			Messenger.Default.Register<int>(this, "ChangeGridSpacing", ChangePixelSnapGrid);
			Messenger.Default.Register<int>(this, "ChangeGridWidth", ChangeGridWidth);
			Messenger.Default.Register<int>(this, "ChangeGridHeight", ChangeGridHeight);
		}

		public void ChangePixelSnapGrid(int gridSpacingSize)
		{
			if (gridSpacingSize == 0)
				gridSpacingSize = 1;

			PixelSnapgrid = gridSpacingSize;
		}

		public void ChangeGridWidth(int width)
		{
			GridWidth = width;
		}

		public void ChangeGridHeight(int height)
		{
			GridHeight = height;
		}

		private void SetImageChangingMessengers()
		{
			Messenger.Default.Register<int>(this, "ChangeLayer", ChangeLayer);
			Messenger.Default.Register<string>(this, "ChangeVisualLayer", ChangeVisualLayer);
			Messenger.Default.Register<int>(this, "ChangeRotate", ChangeRotate);
			Messenger.Default.Register<string>(this, "ChangeVisualRotateSlider",
				ChangeVisualRotateSlider);
			Messenger.Default.Register<float>(this, "ChangeScale", ChangeScale);
			Messenger.Default.Register<string>(this, "ChangeVisualScaleSlider", ChangeVisualScaleSlider);
			Messenger.Default.Register<bool>(this, "ChangeIsButton", ChangeIsButton);
			Messenger.Default.Register<string>(this, "ChangeVisualIsButton", ChangeVisualIsButton);
		}

		public void ChangeIsButton(bool isButton)
		{
			UIImages[SelectedImageInGridIndex].IsButton = isButton;
		}

		public int SelectedImageInGridIndex { get; set; }

		public void ChangeVisualIsButton(string obj)
		{
			if (SelectedImageInGridIndex < 0)
				return;

			CheckBoxButton = UIImages[SelectedImageInGridIndex].IsButton;
			RaisePropertyChanged("CheckBoxButton");
		}

		public bool CheckBoxButton { get; set; }

		public void ChangeLayer(int layer)
		{
			if (SelectedImageInGridIndex < 0 || layer == CheckLayer)
				return;

			UIImages[SelectedImageInGridIndex].Layer = layer;
			UpdateLayers();
		}

		private void UpdateLayers()
		{
			var tempImageLis = new List<UIImage>();
			imageProcessor.SortImagesAccordingToLayer(tempImageLis);
			RaisePropertyChanged("UIImages");
			RaisePropertyChanged("ImageInGridList");
			RaisePropertyChanged("SelectedImageInList");
		}

		public void ChangeVisualLayer(string obj)
		{
			if (SelectedImageInGridIndex < 0)
				return;

			CheckLayer = UIImages[SelectedImageInGridIndex].Layer;
			RaisePropertyChanged("CheckLayer");
		}

		public int CheckLayer { get; set; }

		public void ChangeRotate(int angle)
		{
			Rotate = angle;
			UIImages[SelectedImageInGridIndex].Rotate.Angle = angle;
			RaisePropertyChanged("UIImages");
		}

		public int Rotate { get; set; }

		public void ChangeVisualRotateSlider(string obj)
		{
			if (SelectedImageInGridIndex < 0)
				return;

			Rotate = (int)UIImages[SelectedImageInGridIndex].Rotate.Angle;
			RaisePropertyChanged("Rotate");
		}

		public void ChangeScale(float scale)
		{
			Scale = scale;
			UIImages[SelectedImageInGridIndex].Scale.ScaleX = scale;
			UIImages[SelectedImageInGridIndex].Scale.ScaleY = scale;
			RaisePropertyChanged("UIImages");
		}

		public float Scale { get; set; }

		public void ChangeVisualScaleSlider(string obj)
		{
			if (SelectedImageInGridIndex < 0)
				return;

			Scale = (float)UIImages[SelectedImageInGridIndex].Scale.ScaleX;
			RaisePropertyChanged("Scale");
		}

		private void SetDefaultGridParameters()
		{
			GridWidth = 640;
			GridHeight = 480;
			PixelSnapgrid = 5;
		}

		private void ChangeProject(string project)
		{
			IuImagesInList.Clear();
			string[] imageList = fileSystem.Directory.GetFiles(Path.Combine(ContentPath, project));
			foreach (var imagePath in imageList)
				if (!imagePath.Contains("ContentMetaData.xml"))
					IuImagesInList.Add(Path.GetFileName(imagePath));
			RaisePropertyChanged("IUImagesInList");
		}

		public string ProjectName
		{
			get { return projectName; }
			set
			{
				projectName = value;
				ChangeProject(projectName);
			}
		}

		private string projectName;

		public ObservableCollection<string> ImageInGridList
		{
			get { return imageProcessor.GetNamesOfImagesInGrid(); }
		}

		public string SelectedImageInList { get; set; }
		public BitmapImage ViewImage { get; set; }

		public int GridWidth
		{
			get { return gridWidth; }
			set
			{
				gridWidth = value;
				RaisePropertyChanged("GridWidth");
			}
		}

		private int gridWidth;

		public int GridHeight
		{
			get { return gridHeight; }
			set
			{
				gridHeight = value;
				RaisePropertyChanged("GridHeight");
			}
		}

		private int gridHeight;

		public int PixelSnapgrid
		{
			get { return pixelSnapgrid; }
			set { pixelSnapgrid = value <= 0 ? 1 : value; }
		}

		private int pixelSnapgrid;
		private readonly UIMetaDataSaverAndLoader uiMetaDataSaverAndLoader;

		private void SetSaveAndLoadMessengers()
		{
			Messenger.Default.Register<string>(this, "SaveUI", SaveUIToXml);
			Messenger.Default.Register<string>(this, "LoadUI", LoadUiFromXml);
		}

		public void SaveUIToXml(string filename)
		{
			var root = uiMetaDataSaverAndLoader.CreateMainRoot(filename);
			foreach (var image in UIImages)
				uiMetaDataSaverAndLoader.AddChild(root, image);
			var file = new XmlFile(root);
			string xmlDataString = file.Root.ToXmlString();
			fileSystem.File.WriteAllText(filename, xmlDataString);
		}

		public void LoadUiFromXml(string fileName)
		{
			var file = new XmlFile(fileName);
			uiMetaDataSaverAndLoader.GetImagesFromXmlFile(file, UIImages);
			RaisePropertyChanged("UIImages");
			RaisePropertyChanged("ImageInGridList");
		}
	}
}