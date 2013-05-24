using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions;
using System.Windows.Media.Imaging;
using DeltaEngine.Core.Xml;
using DeltaEngine.Datatypes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;

namespace DeltaEngine.Editor.UIEditor
{
	public class UIEditorViewModel : ViewModelBase
	{
		public UIEditorViewModel(IFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;
			CreateNewLists();
			string[] projects = fileSystem.Directory.GetDirectories(ContentPath);
			foreach (var project in projects)
				ProjectList.Add(Path.GetFileName(project));
			SetMessengerMethods();
			SetDefaultGridParameters();
			uiMetaDataSaverAndLoader = new UIMetaDataSaverAndLoader(fileSystem);
		}

		public IFileSystem fileSystem;
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

			AddNewImageToList();
			RaisePropertyChanged("UIImages");
			RaisePropertyChanged("ImageInGridList");
		}

		private void AddNewImageToList()
		{
			var image = new BitmapImage();
			var fullFilename = Path.Combine(ContentPath, projectName, SelectedImageInList);
			if (!fileSystem.File.Exists(fullFilename))
				throw new FileNotFoundException(fullFilename);

			CreateImageForGrid(fullFilename, image);
			UIImages.Add(new UIImage(0, 0, (float)image.Width, (float)image.Height, image,
				SelectedImageInList, projectName));
			ImageInGridList.Add(SelectedImageInList);
		}

		private void CreateImageForGrid(string fullFilename, BitmapImage image)
		{
			Stream stream = fileSystem.File.OpenRead(fullFilename);
			if (stream.GetType() == typeof(MemoryStream))
				stream = ConvertMemoryStreamToFileStream(stream);

			using (var contentStream = stream)
			{
				image.BeginInit();
				image.CacheOption = BitmapCacheOption.OnLoad;
				image.StreamSource = contentStream;
				image.EndInit();
			}
		}

		public void LeftMouseDown(Point pos)
		{
			LeftMouseButtonDown = true;
			foreach (var image in UIImages)
				if (pos.X > image.X && pos.X < image.X + image.Width && pos.Y > image.Y &&
					pos.Y < image.Y + image.Height)
					SetNewPosition(pos, image);
		}

		public bool LeftMouseButtonDown { get; set; }

		private void SetNewPosition(Point pos, UIImage image)
		{
			SelectedImage = image;
			distance.X = (int)(pos.X - image.X);
			distance.Y = (int)(pos.Y - image.Y);
			imageBeginPos.X = (int)image.X;
			imageBeginPos.Y = (int)image.Y;
		}

		public UIImage SelectedImage { get; set; }
		private Point imageBeginPos;
		private Point distance;

		public void MouseMove(Point pos)
		{
			if (!LeftMouseButtonDown)
				return;

			float posx = (pos.X - distance.X);
			float posy = (pos.Y - distance.Y);
			var gridSpaceX = (int)(posx / PixelSnapgrid);
			var gridSpaceY = (int)(posy / PixelSnapgrid);
			SelectedImage.X = gridSpaceX * PixelSnapgrid;
			SelectedImage.Y = gridSpaceY * PixelSnapgrid;
			RaisePropertyChanged("UIImages");
			RaisePropertyChanged("SelectedImage");
		}

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
			SortImagesAccordingToLayer(tempImageLis);
			string tempFileName = SelectedImageInList;
			SelectedImageInList = tempFileName;
			RaisePropertyChanged("UIImages");
			RaisePropertyChanged("ImageInGridList");
			RaisePropertyChanged("SelectedImageInList");
		}

		private void SortImagesAccordingToLayer(List<UIImage> tempImageLis)
		{
			foreach (var uiImage in UIImages)
				tempImageLis.Add(uiImage);
			tempImageLis.Sort((l, r) => l.Layer.CompareTo(r.Layer));
			UIImages.Clear();
			foreach (var uiImage in tempImageLis)
				UIImages.Add(uiImage);
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
			GridWidth = 50;
			GridHeight = 50;
			PixelSnapgrid = 1;
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
			get { return GetNamesOfImagesInGrid(); }
		}

		private ObservableCollection<string> GetNamesOfImagesInGrid()
		{
			var namelist = new ObservableCollection<string>();
			foreach (var image in UIImages)
				namelist.Add(image.FileName);
			return namelist;
		}

		private static Stream ConvertMemoryStreamToFileStream(Stream stream)
		{
			var file = new FileStream("test.png", FileMode.Create, FileAccess.Write);
			var bytes = new byte[stream.Length];
			stream.Read(bytes, 0, (int)stream.Length);
			file.Write(bytes, 0, bytes.Length);
			file.Close();
			stream.Close();
			stream = file;
			return stream;
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
			Messenger.Default.Register<string>(this, "SaveUI", SaveUI);
			Messenger.Default.Register<string>(this, "LoadUI", LoadUI);
		}

		//ncrunch: no coverage start
		public void SaveUI(string obj)
		{
			var dlg = new SaveFileDialog();
			dlg.FileName = "Document";
			dlg.DefaultExt = ".xml";
			dlg.Filter = "xml documents (.xml)|*.xml";
			bool? result = dlg.ShowDialog();
			if (result == true)
				SaveUIToXml(dlg.FileName);
		}
		//ncrunch: no coverage end

		public void SaveUIToXml(string filename)
		{
			var root = uiMetaDataSaverAndLoader.CreateMainRoot(filename);
			foreach (var image in UIImages)
				uiMetaDataSaverAndLoader.AddChild(root, image);
			var file = new XmlFile(root);
			string xmlDataString = file.Root.ToXmlString();
			fileSystem.File.WriteAllText(filename, xmlDataString);
		}

		//ncrunch: no coverage start
		public void LoadUI(string obj)
		{
			var dlg = new OpenFileDialog();
			dlg.InitialDirectory = "c:\\";
			dlg.Filter = "xml documents (.xml)|*.xml";
			dlg.FilterIndex = 2;
			dlg.RestoreDirectory = true;
			bool? result = dlg.ShowDialog();
			if (result == true)
				LoadUiFromXml(dlg.FileName);
		}
		//ncrunch: no coverage end

		public void LoadUiFromXml(string fileName)
		{
			var file = new XmlFile(fileName);
			uiMetaDataSaverAndLoader.GetImagesFromXmlFile(file, UIImages);
			RaisePropertyChanged("UIImages");
			RaisePropertyChanged("ImageInGridList");
		}
	}
}