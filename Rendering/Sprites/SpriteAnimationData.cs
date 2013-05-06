using System.Collections.Generic;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering.Sprites
{
	/// <summary>
	/// Holds the data used by AnimatedSprite
	/// </summary>
	public class SpriteAnimationData
	{
		public IList<Image> Images { get; set; }
		public float Duration { get; set; }
		public int CurrentFrame { get; set; }
		public float Elapsed { get; set; }
	}
}