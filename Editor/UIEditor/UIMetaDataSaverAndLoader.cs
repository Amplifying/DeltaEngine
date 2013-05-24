using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions;
using System.Windows.Media.Imaging;
using DeltaEngine.Core;
using DeltaEngine.Core.Xml;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Editor.UIEditor
{
	public class UIMetaDataSaverAndLoader
	{
		public UIMetaDataSaverAndLoader(IFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;
		}

		private readonly IFileSystem fileSystem;

		public XmlData CreateMainRoot(string filename)
		{
			var root = new XmlData(filename);
			root.Name = "UIMetaData";
			root.AddAttribute("Name", "DeltaEngine.Editor.UIEditor");
			root.AddAttribute("Type", "Scene");
			root.AddAttribute("LastTimeUpdated", DateTime.Now.GetIsoDateTime());
			root.AddAttribute("ContentDeviceName", "Delta");
			return root;
		}

		public void AddChild(XmlData root, UIImage image)
		{
			var child1 = new XmlData("UIMetaData");
			child1.AddAttribute("PositionX", image.X);
			child1.AddAttribute("PositionY", image.Y);
			child1.AddAttribute("RotationAngle", image.Rotate.Angle);
			child1.AddAttribute("Scale", image.Scale.ScaleX);
			child1.AddAttribute("Layer", image.Layer);
			child1.AddAttribute("SelectedProject", image.Project);
			child1.AddAttribute("IsButton", image.IsButton);
			child1.AddAttribute("FileName", image.FileName);
			root.AddChild(child1);
		}

		public void GetImagesFromXmlFile(XmlFile file, ObservableCollection<UIImage> uiImages)
		{
			uiImages.Clear();
			
			var childrenList = file.Root.Children;
			foreach (var xmlData in childrenList)
				CreateImageOutOfXmlData(xmlData, uiImages);
		}

		private void CreateImageOutOfXmlData(XmlData xmlData, ObservableCollection<UIImage> uiImages)
		{
			var position = GetPosition(xmlData);
			int rotationAngle = Convert.ToInt32(xmlData.GetAttributeValue("RotationAngle"));
			float scale = Convert.ToSingle(xmlData.GetAttributeValue("Scale"));
			int layer = Convert.ToInt32(xmlData.GetAttributeValue("Layer"));
			string selectedProject = xmlData.GetAttributeValue("SelectedProject");
			bool isButton = Convert.ToBoolean(xmlData.GetAttributeValue("IsButton"));
			string fileName = xmlData.GetAttributeValue("FileName");
			var image = CreateLoadedImage(selectedProject, fileName);
			uiImages.Add(new UIImage(position, (float)image.Width, (float)image.Height,
				image, fileName, selectedProject, rotationAngle, scale, layer, isButton));
		}

		private static Point GetPosition(XmlData xmlData)
		{
			int posX = Convert.ToInt32(xmlData.GetAttributeValue("PositionX"));
			int posY = Convert.ToInt32(xmlData.GetAttributeValue("PositionY"));
			var position = new Point(posX, posY);
			return position;
		}

		private BitmapImage CreateLoadedImage(string selectedProject, string fileName)
		{
			var image = new BitmapImage();
			var fullFilename = Path.Combine(UIEditorViewModel.ContentPath, selectedProject, fileName);
			if (!fileSystem.File.Exists(fullFilename))
				throw new FileNotFoundException(fullFilename);

			CreateImageForGrid(fullFilename, image);
			return image;
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
	}
}