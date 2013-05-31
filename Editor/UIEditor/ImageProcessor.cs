using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Editor.UIEditor
{
	public class ImageProcessor
	{
		public ImageProcessor(UIEditorViewModel uiEditorViewModel)
		{
			this.uiEditorViewModel = uiEditorViewModel;
		}

		private readonly UIEditorViewModel uiEditorViewModel;

		public void AddNewImageToList()
		{
			var image = new BitmapImage();
			var fullFilename = Path.Combine(UIEditorViewModel.ContentPath, uiEditorViewModel.ProjectName,
				uiEditorViewModel.SelectedImageInList);
			if (!uiEditorViewModel.fileSystem.File.Exists(fullFilename))
				throw new FileNotFoundException(fullFilename);

			CreateNewImageAndAddToList(fullFilename, image);
		}

		private void CreateNewImageAndAddToList(string fullFilename, BitmapImage image)
		{
			CreateImageForGrid(fullFilename, image);
			var newImage = new UIImage(new Rectangle(0, 0, (float)image.Width, (float)image.Height),
				image, uiEditorViewModel.SelectedImageInList);
			newImage.Project = uiEditorViewModel.ProjectName;
			uiEditorViewModel.UIImages.Add(newImage);
			uiEditorViewModel.ImageInGridList.Add(uiEditorViewModel.SelectedImageInList);
		}

		private void CreateImageForGrid(string fullFilename, BitmapImage image)
		{
			Stream stream = uiEditorViewModel.fileSystem.File.OpenRead(fullFilename);
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

		public ObservableCollection<string> GetNamesOfImagesInGrid()
		{
			var namelist = new ObservableCollection<string>();
			foreach (var image in uiEditorViewModel.UIImages)
				namelist.Add(image.FileName);
			return namelist;
		}

		public void SortImagesAccordingToLayer(List<UIImage> tempImageLis)
		{
			foreach (var uiImage in uiEditorViewModel.UIImages)
				tempImageLis.Add(uiImage);
			tempImageLis.Sort((l, r) => l.Layer.CompareTo(r.Layer));
			uiEditorViewModel.UIImages.Clear();
			foreach (var uiImage in tempImageLis)
				uiEditorViewModel.UIImages.Add(uiImage);
		}

		public void CalculateDistance(Point pos, UIImage image)
		{
			uiEditorViewModel.SelectedImage = image;
			distance = new Point((int)(pos.X - image.X), (int)(pos.Y - image.Y));
		}

		private Point distance;

		public void EditImagePosition(Point pos)
		{
			int pixelSnappingGrid = uiEditorViewModel.PixelSnapgrid;
			float posx = (pos.X - distance.X);
			float posy = (pos.Y - distance.Y);
			var gridSpaceX = (int)(posx / pixelSnappingGrid);
			var gridSpaceY = (int)(posy / pixelSnappingGrid);
			uiEditorViewModel.SelectedImage.X = gridSpaceX * pixelSnappingGrid;
			uiEditorViewModel.SelectedImage.Y = gridSpaceY * pixelSnappingGrid;
		}
	}
}