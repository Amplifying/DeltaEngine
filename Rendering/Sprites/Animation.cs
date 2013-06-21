using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering.Sprites
{
	/// <summary>
	/// An animated sprite
	/// </summary>
	public class Animation : Sprite
	{
		public Animation(IList<Image> images, Rectangle initialDrawArea, float animationLength)
			: base(images[0], initialDrawArea)
		{
			Add(new Data { Images = images, Duration = animationLength });
			Add<Update>();
		}

		public Animation(string animationName, ContentLoader contentLoader, Rectangle initialDrawArea,
			float animationLength) :
			base(contentLoader.LoadRecursively<Image>(animationName)[0], initialDrawArea)
		{
			var images = contentLoader.LoadRecursively<Image>(animationName);
			Add(new Data { Images = images, Duration = animationLength });
			Add<Update>();

		}

		public IList<Image> Images
		{
			get { return Get<Data>().Images; }
			set { Get<Data>().Images = value; }
		}

		public float Duration
		{
			get { return Get<Data>().Duration; }
			set { Get<Data>().Duration = value; }
		}

		public int CurrentFrame
		{
			get { return Get<Data>().CurrentFrame; }
		}

		public void AddImageWithoutIncreasingDuration(Image image)
		{
			Get<Data>().Images.Add(image);
		}

		public void AddImageIncreasingDuration(Image image)
		{
			var animationData = Get<Data>();
			animationData.Images.Add(image);
			float imageCount = animationData.Images.Count;
			animationData.Duration *= imageCount / (imageCount - 1);
		}

		public void Reset()
		{
			var animationData = Get<Data>();
			animationData.CurrentFrame = 0;
			animationData.Elapsed = 0;
		}

		/// <summary>
		/// Holds the data used by AnimatedSprite
		/// </summary>
		public class Data
		{
			public IList<Image> Images { get; set; }
			public float Duration { get; set; }
			public int CurrentFrame { get; set; }
			public float Elapsed { get; set; }
		}

		/// <summary>
		/// Updates current frame for a sprite animation
		/// </summary>
		public class Update : EntityHandler
		{
			public override void Handle(Entity entity)
			{
				var animationData = entity.Get<Data>();
				animationData.Elapsed += Time.Current.Delta;
				animationData.Elapsed = animationData.Elapsed % animationData.Duration;
				animationData.CurrentFrame =
					(int)(animationData.Images.Count * animationData.Elapsed / animationData.Duration);
				entity.Set(animationData.Images[animationData.CurrentFrame]);
			}

			public override EntityHandlerPriority Priority
			{
				get { return EntityHandlerPriority.First; }
			}
		}
	}
}