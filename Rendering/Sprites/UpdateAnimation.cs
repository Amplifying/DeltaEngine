using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering.Sprites
{
	/// <summary>
	/// Updates current frame for a sprite animation
	/// </summary>
	public class UpdateAnimation : EntityHandler
	{
		public void Handle(List<Entity> entities)
		{
			foreach (Entity entity in entities.Where(e => e.Contains<Image, SpriteAnimationData>()))
				UpdateCurrentFrame(entity);
		}

		private static void UpdateCurrentFrame(Entity entity)
		{
			var animationData = entity.Get<SpriteAnimationData>();
			animationData.Elapsed += Time.Current.Delta;
			animationData.Elapsed = animationData.Elapsed % animationData.Duration;
			animationData.CurrentFrame =
				(int)(animationData.Images.Count * animationData.Elapsed / animationData.Duration);
			entity.Set(animationData.Images[animationData.CurrentFrame]);
		}

		public EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.First; }
		}
	}
}