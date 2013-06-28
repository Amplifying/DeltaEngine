using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Content.Xml;
using DeltaEngine.Core;

namespace DeltaEngine.Editor.Mocks
{
	/// <summary>
	/// helps create the ContentMetaDataFiles
	/// </summary>
	public class ContentMetaDataCreator
	{
		public ContentMetaDataCreator(ContentServiceFiles content)
		{
			this.content = content;
		}

		private ContentServiceFiles content;

		public void SaveImagesToXml(string selectedProject)
		{
			var root = CreateMainContentMetaData(selectedProject, "ContentMetaData");
			foreach (var image in content.ImageList)
				AddChild(root, image, selectedProject, "ContentMetaData");
			var file = new XmlFile(root);
			string path = Path.Combine(content.ContentPath, selectedProject, MetaDataFilename);
			string xmlDataString = file.Root.ToXmlString();
			content.fileSystem.File.WriteAllText(path, xmlDataString);
		}

		private const string MetaDataFilename = "ContentMetaData.xml";

		private static XmlData CreateMainContentMetaData(string selectedProject, string name)
		{
			var root = new XmlData(selectedProject);
			root.Name = name;
			root.AddAttribute("Name", "DeltaEngine.Editor.ContentManager");
			root.AddAttribute("Type", "Scene");
			root.AddAttribute("LastTimeUpdated", DateTime.Now.GetIsoDateTime());
			root.AddAttribute("ContentDeviceName", "Delta");
			return root;
		}

		private void AddChild(XmlData root, string image, string selectedProject, string name)
		{
			if (image.Contains("ContentMetaData.xml"))
				return;

			string fileName = Path.GetFileNameWithoutExtension(image);
			string fileType = Path.GetExtension(image);
			var child1 = new XmlData(name);
			AddAttributesToChild(image, selectedProject, child1, fileName, fileType);
			root.AddChild(child1);
		}

		private void AddAttributesToChild(string image, string selectedProject, XmlData child1,
			string fileName, string fileType)
		{
			child1.AddAttribute("Name", fileName);
			child1.AddAttribute("Type", fileType);
			child1.AddAttribute("LastTimeUpdated", DateTime.Now.GetIsoDateTime());
			child1.AddAttribute("PlatformFileId", 1);
			string fullPath = Path.Combine(content.ContentPath, selectedProject, image);
			child1.AddAttribute("FileSize", content.fileSystem.FileInfo.FromFileName(fullPath).Length);
			child1.AddAttribute("LocalFilePath", image);
		}

		public void SetContent(ContentServiceFiles newContent)
		{
			content = newContent;
		}

		public void SaveImagesAsAnimation(List<string> itemList, string animationName,
			string selectedProject)
		{
			var root = CreateMainContentMetaData(selectedProject, animationName);
			foreach (var item in itemList)
				AddChild(root, item, selectedProject, animationName);
			var file = new XmlFile(root);
			string path = Path.Combine(content.ContentPath, selectedProject, animationName + ".xml");
			string xmlDataString = file.Root.ToXmlString();
			content.fileSystem.File.WriteAllText(path, xmlDataString);
		}
	}
}