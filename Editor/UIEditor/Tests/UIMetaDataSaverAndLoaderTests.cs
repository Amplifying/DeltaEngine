using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using DeltaEngine.Core.Xml;
using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Editor.UIEditor.Tests
{
	internal class UIMetaDataSaverAndLoaderTests
	{
		[SetUp]
		public void CreateUIEditorViewModel()
		{
			var fileSystem =
				new MockFileSystem(new Dictionary<string, MockFileData>
				{
					{
						@"Content\BreakOut\DeltaEngineLogo.png",
						new MockFileData(DataToString(@"Content\DeltaEngineLogo.png"))
					}
				});
			saverAndLoader = new UIMetaDataSaverAndLoader(fileSystem);
		}

		private UIMetaDataSaverAndLoader saverAndLoader;

		private static string DataToString(string path)
		{
			var fileSystem = new FileSystem();
			return fileSystem.File.ReadAllText(path);
		}

		[Test]
		public void LoadXml()
		{
			string xmlPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string filePath = Path.Combine(xmlPath, "Content", "TestXml.xml");
			var file = new XmlFile(filePath);
			var imageList = new ObservableCollection<UIImage>();
			saverAndLoader.GetImagesFromXmlFile(file, imageList);
		}

		[Test]
		public void SetSaveData()
		{
			string xmlPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string filePath = Path.Combine(xmlPath, "Content", "TestXml.xml");
			var file = new XmlFile(filePath);
			var imageList = new ObservableCollection<UIImage>();
			saverAndLoader.GetImagesFromXmlFile(file, imageList);
			var image = new UIImage(new Rectangle(0, 0, 1, 1), imageList[0].BitmapSource, "Test");
			image.Project = "BreakOut";
			var root = saverAndLoader.CreateMainRoot("testFile.xml");
			saverAndLoader.AddChild(root, image);
		}
	}
}