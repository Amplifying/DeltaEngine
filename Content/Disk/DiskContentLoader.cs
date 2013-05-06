using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using DeltaEngine.Core;

namespace DeltaEngine.Content.Disk
{
	/// <summary>
	/// Implementation of the ContentLoader that loads and caches files directly from disk.
	/// </summary>
	public class DiskContentLoader : ContentLoader
	{
		public DiskContentLoader(ContentDataResolver resolver)
			: base(resolver)
		{
			contentPath = "Content";
			MakeSureContentPathExists();
			TryCreateMetaDataIfNonExistant();
			LoadMetaData();
		}

		private void MakeSureContentPathExists()
		{
			if (!Directory.Exists(ContentPath))
				Console.WriteLine("Content directory does not exist, no content available.");
		}

		public bool SetPathIfAcceptable(string newContentPath)
		{
			if (!Uri.IsWellFormedUriString(newContentPath, UriKind.RelativeOrAbsolute))
			{
				Console.WriteLine("The given string \"" + newContentPath + "\" is not a valid filepath!");
				return false;
			}

			contentPath = newContentPath;
			if (!Directory.Exists(contentPath))
				Directory.CreateDirectory(contentPath);
			return true;
		}

		private string contentPath;

		public string ContentPath
		{
			get { return contentPath; }
		}

		private void TryCreateMetaDataIfNonExistant()
		{
			if (File.Exists(ContentMetaDataFilePath))
				return;

			Console.WriteLine("No ContentMetaData.xml found. I shall prepare a new one for you. ^^");
			WriteMetaDataDocument();
		}

		private string ContentMetaDataFilePath
		{
			get { return Path.Combine(ContentPath, "ContentMetaData.xml"); }
		}

		private void WriteMetaDataDocument()
		{
			using (var writer = CreateXmlFile(ContentMetaDataFilePath))
			{
				writer.WriteStartDocument();
				writer.WriteComment(
					"Delta Engine Content Meta Data, auto-generated, will be refreshed at each start");
				writer.WriteStartElement("ContentMetaData");
				WriteAttribute(writer, "Name", AssemblyExtensions.DetermineProjectName());
				WriteAttribute(writer, "Type", "Scene");
				WriteAttribute(writer, "LastTimeUpdated",
					Directory.GetLastWriteTime(Directory.GetCurrentDirectory()).ToString("u"));
				WriteAttribute(writer, "ContentDeviceName", "Delta");
				foreach (var file in Directory.GetFiles(ContentPath))
					if (!file.Contains("ContentMetaData.xml") && !file.Contains("thumbs.db"))
						CreateContentMetaDataEntry(writer, file);
				writer.WriteEndElement();
			}
		}

		private static XmlTextWriter CreateXmlFile(string metaDataFilePath)
		{
			Stream metadataStream = File.Create(metaDataFilePath);
			return new XmlTextWriter(metadataStream, new UTF8Encoding())
			{
				Formatting = Formatting.Indented
			};
		}

		private static void WriteAttribute(XmlWriter writer, string name, string value)
		{
			writer.WriteStartAttribute(name);
			writer.WriteValue(value);
			writer.WriteEndAttribute();
		}

		private void CreateContentMetaDataEntry(XmlWriter writer, string filePath)
		{
			writer.WriteStartElement("ContentMetaData");
			WriteAttribute(writer, "Name", Path.GetFileNameWithoutExtension(filePath));
			WriteAttribute(writer, "Type", ExtensionToType(Path.GetExtension(filePath)).ToString());
			var info = new FileInfo(filePath);
			WriteAttribute(writer, "LastTimeUpdated", info.LastWriteTime.ToString("u"));
			WriteAttribute(writer, "PlatformFileId", "-" + (++generatedContentCounter));
			WriteAttribute(writer, "FileSize", "" + info.Length);
			WriteAttribute(writer, "LocalFilePath", Path.GetFileName(filePath));
			writer.WriteEndElement();
		}

		private static ContentType ExtensionToType(string extension)
		{
			switch (extension.ToLower())
			{
			case ".png":
			case ".jpg":
			case ".bmp":
			case ".tif":
				return ContentType.Image;
			case ".wav":
				return ContentType.Sound;
			case ".mp3":
			case ".ogg":
				return ContentType.Music;
			case ".mp4":
				return ContentType.Video;
			case ".xml":
				return ContentType.Xml;
			}
			throw new UnsupportedContentFileFoundCannotParseType(extension);
		}

		private int generatedContentCounter;

		private class UnsupportedContentFileFoundCannotParseType : Exception
		{
			public UnsupportedContentFileFoundCannotParseType(string extension)
				: base(extension) {}
		}

		private void LoadMetaData()
		{
			var xml = XDocument.Load(ContentMetaDataFilePath);
			if (xml.Nodes().OfType<XComment>().Any())
			{
				WriteMetaDataDocument();
				xml = XDocument.Load(ContentMetaDataFilePath);
			}
			ParseXmlNode(xml.Root);
		}

		private void ParseXmlNode(XElement parent)
		{
			if (parent.Attributes("Name").Any() && parent.Attributes("LocalFilePath").Any())
				contentFilenames.Add(parent.Attribute("Name").Value,
					parent.Attribute("LocalFilePath").Value);

			foreach (var node in parent.Elements())
				ParseXmlNode(node);
		}

		protected override Stream GetContentDataStream(string contentName)
		{
			var filePath = Path.Combine(ContentPath, GetFilenameFromContentName(contentName));
			try
			{
				return File.OpenRead(filePath);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Requested content file " + filePath +
					" does not exist or cannot be accessed.");
				if (!Debugger.IsAttached)
					throw new ContentFileDoesNotExistOrIsInaccessible(filePath, ex);

				return Stream.Null;
			}
		}

		public class ContentFileDoesNotExistOrIsInaccessible : Exception
		{
			public ContentFileDoesNotExistOrIsInaccessible(string filePath, Exception innerException)
				: base(filePath, innerException) {}
		}
	}
}