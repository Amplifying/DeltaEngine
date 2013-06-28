using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering.Sprites
{
	/// <summary>
	/// ContentData representing a sequence of images used in a Sprite-Animation.
	/// Its ContentMetaData-Entry should contain among its children at least one Image
	/// and nothing but Images.
	/// </summary>
	public class AnimationData : ContentData
	{
		public AnimationData(string contentName)
			: base(contentName)
		{
			Frames = new List<Image>();
		}

		public List<Image> Frames { get; private set; }

		protected override void DisposeData()
		{
			foreach (Image frame in Frames)
				frame.Dispose();
		}

		protected override void LoadData(Stream fileData)
		{
			var imageNames = MetaData.GetChildrenNames(ContentType.Image);
			if (imageNames.Count == 0)
				throw new NoImagesForAnimationPresent(Name);

			foreach (var image in imageNames)
				Frames.Add(ContentLoader.Load<Image>(image));

			Duration = MetaData.Get("Duration", 0.0f);
		}

		public class NoImagesForAnimationPresent : Exception
		{
			public NoImagesForAnimationPresent(string contentName)
				: base(contentName) {}
		}

		public float Duration { get; set; }
	}
}