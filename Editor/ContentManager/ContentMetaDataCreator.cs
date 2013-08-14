using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor.ContentManager
{
	/// <summary>
	/// Used by ContentManager to generate MetaData to be sent via UpdateContent to the OnlineService
	/// </summary>
	public class ContentMetaDataCreator
	{
		public ContentMetaDataCreator(Service service)
		{
			this.service = service;
		}

		private readonly Service service;

		//ncrunch: no coverage start
		public ContentMetaData CreateMetaDataFromFile(string filePath)
		{
			var contentMetaData = new ContentMetaData();
			contentMetaData.Name = Path.GetFileNameWithoutExtension(filePath);
			contentMetaData.Type = ExtensionToType(Path.GetExtension(filePath));
			contentMetaData.LastTimeUpdated = File.GetLastWriteTime(filePath);
			contentMetaData.Language = "en";
			contentMetaData.LocalFilePath = Path.GetFileName(filePath);
			contentMetaData.PlatformFileId = 0;
			contentMetaData.FileSize = (int)new FileInfo(filePath).Length;
			if (contentMetaData.Type == ContentType.Image)
				AddImageDataFromBitmapToContentMetaData(filePath, contentMetaData);
			return contentMetaData;
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
			case ".gif":
				return ContentType.JustStore;
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
			case ".deltamesh":
				return ContentType.Mesh;
			case ".deltaparticle":
				return ContentType.ParticleEffect;
			case ".deltashader":
				return ContentType.Shader;
			case ".deltamaterial":
				return ContentType.Material;
			}
			throw new UnsupportedContentFileFoundCannotParseType(extension);
		}

		private class UnsupportedContentFileFoundCannotParseType : Exception
		{
			public UnsupportedContentFileFoundCannotParseType(string extension)
				: base(extension) {}
		}

		private static void AddImageDataFromBitmapToContentMetaData(string filePath,
			ContentMetaData metaData)
		{
			try
			{
				var bitmap = new Bitmap(filePath);
				metaData.Values.Add("PixelSize", "(" + bitmap.Width + "," + bitmap.Height + ")");
				metaData.Values.Add("BlendMode", HasBitmapAlphaPixels(bitmap) ? "Normal" : "Opaque");
			}
			catch (Exception)
			{
				throw new UnknownImageFormatUnableToAquirePixelSize(filePath);
			}
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

		private class UnknownImageFormatUnableToAquirePixelSize : Exception
		{
			public UnknownImageFormatUnableToAquirePixelSize(string message)
				: base(message) {}
		}

		public ContentMetaData CreateMetaDataFromImageAnimation(string animationName,
			ImageAnimation animation)
		{
			var contentMetaData = new ContentMetaData();
			contentMetaData.Name = animationName;
			contentMetaData.Type = ContentType.ImageAnimation;
			contentMetaData.LastTimeUpdated = DateTime.Now;
			contentMetaData.Language = "en";
			contentMetaData.PlatformFileId = 0;
			contentMetaData.Values.Add("DefaultDuration", animation.DefaultDuration.ToString());
			string images = "";
			for (int index = 0; index < animation.Frames.Length; index++)
			{
				var image = animation.Frames[index];
				if (images == "")
					images += (image.Name);
				else
					images += (", " + image.Name);
			}
			contentMetaData.Values.Add("ImageNames", images);
			return contentMetaData;
		}

		public ContentMetaData CreateMetaDataFromSpriteSheetAnimation(string animationName,
			SpriteSheetAnimation spriteSheetAnimation)
		{
			var contentMetaData = new ContentMetaData();
			contentMetaData.Name = animationName;
			contentMetaData.Type = ContentType.SpriteSheetAnimation;
			contentMetaData.LastTimeUpdated = DateTime.Now;
			contentMetaData.Language = "en";
			contentMetaData.PlatformFileId = 0;
			contentMetaData.Values.Add("DefaultDuration",
				spriteSheetAnimation.DefaultDuration.ToString());
			contentMetaData.Values.Add("SubImageSize", spriteSheetAnimation.SubImageSize.ToString());
			contentMetaData.Values.Add("ImageName", spriteSheetAnimation.Image.Name);
			return contentMetaData;
		}

		public ContentMetaData CreateMetaDataFromParticle(string particleName, byte[] byteArray)
		{
			var contentMetaData = new ContentMetaData();
			contentMetaData.Name = particleName;
			contentMetaData.Type = ContentType.ParticleEffect;
			contentMetaData.LocalFilePath = particleName + ".deltaparticle";
			contentMetaData.LastTimeUpdated = DateTime.Now;
			contentMetaData.Language = "en";
			contentMetaData.PlatformFileId = 0;
			contentMetaData.FileSize = byteArray.Length;
			return contentMetaData;
		}

		public ContentMetaData CreateMetaDataFromMaterial(string materialName, Material material)
		{
			var contentMetaData = new ContentMetaData();
			contentMetaData.Name = materialName;
			contentMetaData.Type = ContentType.Material;
			contentMetaData.LastTimeUpdated = DateTime.Now;
			contentMetaData.Language = "en";
			contentMetaData.PlatformFileId = 0;
			contentMetaData.Values.Add("ShaderName", material.Shader.Name);
			if (material.Animation != null)
				contentMetaData.Values.Add("ImageOrAnimationName", material.Animation.Name);
			else if (material.SpriteSheet != null)
				contentMetaData.Values.Add("ImageOrAnimationName", material.SpriteSheet.Name);
			else
				contentMetaData.Values.Add("ImageOrAnimationName", material.DiffuseMap.Name);
			contentMetaData.Values.Add("Color", material.DefaultColor.ToString());
			return contentMetaData;
		}
	}
}