using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering.Sprites
{
	/// <summary>
	/// A Physics Sprite that falls under gravity
	/// </summary>
	public class FallingSprite : PhysicsSprite
	{
		public FallingSprite(Image image, Rectangle drawArea, float duration = 10.0f)
			: base(image, drawArea)
		{
			Add<Fall>();
			PhysicsDuration = duration;
		}
	}
}
