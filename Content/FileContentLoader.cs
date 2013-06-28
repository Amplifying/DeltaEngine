using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DeltaEngine.Logging;

namespace DeltaEngine.Content
{
	/// <summary>
	/// Loads and caches files directly from disk using an xml file created earlier by ContentManager
	/// to get all content meta data (names, types, last time updated, pixel size, etc.)
	/// </summary>
	public class FileContentLoader : ContentLoader
	{
		public FileContentLoader(ContentDataResolver resolver, string contentPath = "Content")
			: base(resolver, contentPath) {}

		protected override void LazyInitialize()
		{
			if (isInitialized)
				return;

			isInitialized = true;
			if (!Directory.Exists(ContentPath))
				throw new DirectoryNotFoundException("Content directory " + ContentPath +
					" does not exist, unable to contine");

			LoadMetaData(Path.Combine(ContentPath, "ContentMetaData.xml"));
		}

		protected bool isInitialized;

		private void LoadMetaData(string xmlFilePath)
		{
			if (IsMetaDataNotLongerUpToDate(xmlFilePath))
				xml = new ContentMetaDataFileCreator(xml).CreateAndLoad(xmlFilePath);
			ParseXmlNode(xml.Root);
		}

		private XDocument xml;

		private bool IsMetaDataNotLongerUpToDate(string filePath)
		{
			if (File.Exists(filePath))
			{
				using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					xml = XDocument.Load(fs);
				return xml.Nodes().OfType<XComment>().Any() &&
					(DateTime.Now - new FileInfo(filePath).LastWriteTime).TotalSeconds > 90;
			}

			Logger.Current.Info("ContentMetaData.xml not found, a new one will be created. " +
				"For proper content meta data please use the Delta Engine Editor.");
			return true;
		}

		private void ParseXmlNode(XElement current, ContentMetaData parent = null)
		{
			var currentElement = new ContentMetaData(current.Attributes());
			if (parent != null)
				parent.AddChildren(currentElement);

			var name = current.Attribute("Name").Value;
			if(!metaData.ContainsKey(name))
				metaData.Add(name, currentElement);

			foreach (var node in current.Elements())
				ParseXmlNode(node, currentElement);
		}

		private readonly Dictionary<string, ContentMetaData> metaData =
			new Dictionary<string, ContentMetaData>(StringComparer.OrdinalIgnoreCase);

		protected override ContentMetaData GetMetaData(string contentName)
		{
			LazyInitialize();
			return metaData.ContainsKey(contentName) ? metaData[contentName] : null;
		}

		protected override Stream GetContentDataStream(ContentData content)
		{
			if (String.IsNullOrEmpty(content.MetaData.LocalFilePath))
				return Stream.Null;

			var filePath = Path.Combine(ContentPath, content.MetaData.LocalFilePath);
			try
			{
				return File.OpenRead(filePath);
			}
			catch (Exception ex)
			{
				Logger.Current.Error(new ContentFileDoesNotExist(filePath, ex));
				if (!Debugger.IsAttached)
					throw new ContentFileDoesNotExist(filePath, ex);

				return Stream.Null;
			}
		}

		public class ContentFileDoesNotExist : Exception
		{
			public ContentFileDoesNotExist(string filePath, Exception innerException)
				: base(filePath, innerException) {}
		}
	}
}