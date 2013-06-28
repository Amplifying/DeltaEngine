using System;
using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering.Sprites
{
	/// <summary>
	/// An animated sprite consisting of multiple images
	/// </summary>
	public class Animation : Sprite
	{
		public Animation(string name, Rectangle drawArea)
			: this(ContentLoader.Load<AnimationData>(name), drawArea) {}

		private Animation(AnimationData data, Rectangle drawArea)
			: base(data.Frames[0], drawArea)
		{
			Add(data);
			Elapsed = 0.0f;
			CurrentFrame = 0;
			IsPlaying = true;
			Start<Update>();
		}

		public float Elapsed { get; set; }
		public int CurrentFrame { get; private set; }
		public bool IsPlaying { get; set; }

		public IList<Image> Images
		{
			get { return Get<AnimationData>().Frames; }
		}

		public float Duration
		{
			get { return Get<AnimationData>().Duration; }
			set { Get<AnimationData>().Duration = value; }
		}

		public void Reset()
		{
			CurrentFrame = 0;
			Elapsed = 0.0f;
			Set(Get<AnimationData>().Frames[CurrentFrame]);
		}

		internal void InvokeFinalFrame()
		{
			if (FinalFrame != null)
				FinalFrame();
		}

		public event Action FinalFrame;

		/// <summary>
		/// Updates current frame for a sprite animation
		/// </summary>
		public class Update : Behavior2D
		{
			public override void Handle(Entity2D entity)
			{
				var animation = entity as Animation;
				if (!animation.IsPlaying)
					return;
				var animationData = animation.Get<AnimationData>();
				animation.Elapsed += Time.Current.Delta;
				animation.Elapsed = animation.Elapsed % animationData.Duration;
				animation.CurrentFrame =
					(int)(animationData.Frames.Count * animation.Elapsed / animationData.Duration);
				entity.Set(animationData.Frames[animation.CurrentFrame]);
				if (animation.CurrentFrame == animationData.Frames.Count - 1)
					animation.InvokeFinalFrame();
			}

			public override Priority Priority
			{
				get { return Priority.First; }
			}
		}
	}
}