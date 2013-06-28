using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using DeltaEngine.Core;

namespace DeltaEngine.Content
{
	/// <summary>
	/// Creates the ContentMetaData File to be used in ContentLoader based on the content files.
	/// </summary>
	public class ContentMetaDataFileCreator
	{
		public ContentMetaDataFileCreator(XDocument lastXml)
		{
			this.lastXml = lastXml;
		}

		private readonly XDocument lastXml;

		public XDocument CreateAndLoad(string xmlFilePath)
		{
			filePath = xmlFilePath;
			WriteMetaDataDocument();
			return XDocument.Load(filePath);
		}

		private string filePath;

		private void WriteMetaDataDocument()
		{
			using (var writer = CreateMetaDataXmlFile())
			{
				writer.WriteStartDocument();
				writer.WriteComment(
					"Delta Engine Content Meta Data, auto-generated, will be refreshed at each start");
				writer.WriteStartElement("ContentMetaData");
				WriteAttribute(writer, "Name", AssemblyExtensions.GetEntryAssemblyForProjectName());
				WriteAttribute(writer, "Type", "Scene");
				WriteAttribute(writer, "LastTimeUpdated",
					Directory.GetLastWriteTime(Directory.GetCurrentDirectory()).ToString("u"));
				WriteAttribute(writer, "ContentDeviceName", "Delta");
				foreach (var file in Directory.GetFiles(Path.GetDirectoryName(filePath)))
					if (!IsFilenameIgnored(Path.GetFileName(file)))
						CreateContentMetaDataEntry(writer, file);
				writer.WriteEndElement();
			}
		}

		private XmlTextWriter CreateMetaDataXmlFile()
		{
			Stream stream = File.Create(filePath);
			return new XmlTextWriter(stream, new UTF8Encoding()) { Formatting = Formatting.Indented };
		}

		private static void WriteAttribute(XmlWriter writer, string name, string value)
		{
			writer.WriteStartAttribute(name);
			writer.WriteValue(value);
			writer.WriteEndAttribute();
		}

		private bool IsFilenameIgnored(string fileName)
		{
			return ignoredFiles.Contains(Path.GetFileName(fileName).ToLower()) ||
				fileName.ToLower().EndsWith(".xnb");
		}

		private readonly List<string> ignoredFiles =
			new List<string>(new[] { "contentmetadata.xml", "thumbs.db", "desktop.ini", ".ds_store" });

		private void CreateContentMetaDataEntry(XmlWriter writer, string contentFile)
		{
			writer.WriteStartElement("ContentMetaData");
			var contentType = ExtensionToType(Path.GetExtension(contentFile));
			WriteAttribute(writer, "Name", Path.GetFileNameWithoutExtension(contentFile));
			WriteAttribute(writer, "Type", contentType.ToString());
			WriteMetaData(writer, contentFile, contentType);
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
			case ".wma":
				return ContentType.Music;
			case ".mp4":
			case ".avi":
			case ".wmv":
				return ContentType.Video;
			case ".xml":
				return ContentType.Xml;
			case ".json":
				return ContentType.Json;
			case ".obj":
				return ContentType.Mesh;
			}
			throw new UnsupportedContentFileFoundCannotParseType(extension);
		}

		private class UnsupportedContentFileFoundCannotParseType : Exception
		{
			public UnsupportedContentFileFoundCannotParseType(string extension)
				: base(extension) {}
		}

		private void WriteMetaData(XmlWriter writer, string contentFile, ContentType type)
		{
			var info = new FileInfo(contentFile);
			WriteAttribute(writer, "LastTimeUpdated", info.LastWriteTime.ToString("u"));
			WriteAttribute(writer, "PlatformFileId", --generatedPlatformFileId + "");
			WriteAttribute(writer, "FileSize", "" + info.Length);
			WriteAttribute(writer, "LocalFilePath", Path.GetFileName(contentFile));
			if (type == ContentType.Image)
				GeneratePixelSizeAndBlendModeIfNeeded(writer, contentFile);
		}

		private int generatedPlatformFileId;

		private void GeneratePixelSizeAndBlendModeIfNeeded(XmlWriter writer, string imageFilePath)
		{
			if (lastXml != null &&
				CanWriteImageDataFromLastXml(writer, lastXml.Root, Path.GetFileName(imageFilePath)))
				return;

			TryWriteImageDataFromBitmap(writer, imageFilePath);
		}

		private static bool CanWriteImageDataFromLastXml(XmlWriter writer, XElement element,
			string imageFilename)
		{
			foreach (var child in element.Elements())
			{
				if (child.Attribute("LocalFilePath") != null &&
					child.Attribute("LocalFilePath").Value == imageFilename &&
					child.Attribute("PixelSize") != null &&
					child.Attribute("BlendMode") != null)
				{
					WriteAttribute(writer, "PixelSize", child.Attribute("PixelSize").Value);
					WriteAttribute(writer, "BlendMode", child.Attribute("BlendMode").Value);
					return true;
				}

				if (CanWriteImageDataFromLastXml(writer, child, imageFilename))
					return true;
			}
			return false;
		}

		private static void TryWriteImageDataFromBitmap(XmlWriter writer, string filePath)
		{
			try
			{
				var bitmap = new Bitmap(filePath);
				WriteAttribute(writer, "PixelSize", "(" + bitmap.Width + ", " + bitmap.Height + ")");
				WriteAttribute(writer, "BlendMode", HasBitmapAlphaPixels(bitmap) ? "Normal" : "Opaque");
			}
			catch (Exception)
			{
				throw new UnknownImageFormatUnableToAquirePixelSize(filePath);
			}
		}

		private class UnknownImageFormatUnableToAquirePixelSize : Exception
		{
			public UnknownImageFormatUnableToAquirePixelSize(string message)
				: base(message) {}
		}

		private static unsafe bool HasBitmapAlphaPixels(Bitmap bitmap)
		{
			int width = bitmap.Width;
			int height = bitmap.Height;
			var bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
				PixelFormat.Format32bppArgb);
			var bitmapPointer = (byte*)bitmapData.Scan0.ToPointer();
			var foundAlphaPixel = HasImageDataAlpha(width, height, bitmapPointer);
			bitmap.UnlockBits(bitmapData);
			return foundAlphaPixel;
		}

		private static unsafe bool HasImageDataAlpha(int width, int height, byte* bitmapPointer)
		{
			for (int y = 0; y < height; ++y)
				for (int x = 0; x < width; ++x)
					if (bitmapPointer[(y * width + x) * 4 + 3] != 255)
						return true;

			return false;
		}
	}
}