using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering.Sprites
{
	/// <summary>
	/// AnimatedSprites are 2D quads rendered automatically on the specified location
	/// changing the main image for the next one in a determined fps, etc.
	/// </summary>
	public class AnimatedSprite : Sprite
	{
		public AnimatedSprite(IList<Image> images, Rectangle initialDrawArea, float animationLength)
			: base(images[0], initialDrawArea)
		{
			Add(new SpriteAnimationData { Images = images, Duration = animationLength });
			Add<UpdateAnimation>();
		}

		public IList<Image> Images
		{
			get { return Get<SpriteAnimationData>().Images; }
			set { Get<SpriteAnimationData>().Images = value; }
		}

		public float Duration
		{
			get { return Get<SpriteAnimationData>().Duration; }
			set { Get<SpriteAnimationData>().Duration = value; }
		}

		public int CurrentFrame
		{
			get { return Get<SpriteAnimationData>().CurrentFrame; }
		}

		public void AddImageWithoutIncreasingDuration(Image image)
		{
			Get<SpriteAnimationData>().Images.Add(image);
		}

		public void AddImageIncreasingDuration(Image image)
		{
			var animationData = Get<SpriteAnimationData>();
			animationData.Images.Add(image);
			float imageCount = animationData.Images.Count;
			animationData.Duration *= imageCount / (imageCount - 1);
		}

		public void Reset()
		{
			var animationData = Get<SpriteAnimationData>();
			animationData.CurrentFrame = 0;
			animationData.Elapsed = 0;
		}
	}
}