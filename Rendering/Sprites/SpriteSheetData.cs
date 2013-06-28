using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering.Sprites
{
	public class SpriteSheetData : ContentData
	{
		public SpriteSheetData(string contentName)
			: base(contentName) {}

		protected override void DisposeData()
		{
			if (IsDisposed)
				Image.Dispose();
		}

		public Image Image { get; private set; }

		protected override void LoadData(Stream fileData)
		{
			var images = MetaData.GetChildrenNames(ContentType.Image);
			if (images.Count != 1)
				throw new WrongNumberOfChildrenForSpriteSheet();

			foreach (var image in images)
				Image = ContentLoader.Load<Image>(image);

			GetAttributes();
			CreateUVs();
		}

		public class WrongNumberOfChildrenForSpriteSheet : Exception {}

		private void GetAttributes()
		{
			Duration = MetaData.Get("Duration", 0.0f);
			SubImageSize = MetaData.Get("SubImageSize", Size.Zero);
			if (SubImageSize == Size.Zero)
				throw new SubImageSizeNeededForSpriteSheed();
		}

		public Size SubImageSize { get; private set; }

		private class SubImageSizeNeededForSpriteSheed : Exception {}

		private void CreateUVs()
		{
			UVs = new List<Rectangle>();
			for (int x = 0; x < Image.PixelSize.Width / SubImageSize.Width; x++)
				for (int y = 0; y < Image.PixelSize.Height / SubImageSize.Height; y++)
					UVs.Add(Rectangle.BuildUvRectangle(CalculatePixelRect(x, y), Image.PixelSize));
		}

		public List<Rectangle> UVs { get; private set; }

		private Rectangle CalculatePixelRect(int x, int y)
		{
			return new Rectangle(x * SubImageSize.Width, y * SubImageSize.Height, SubImageSize.Width,
				SubImageSize.Height);
		}

		public float Duration { get; set; }
	}
}