using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;

namespace GameOfDeath
{
	/// <summary>
	/// Dead rabbits are just simple fadeout effects and have no meaning to the game play.
	/// </summary>
	public class DeadRabbit : FadeoutEffect
	{
		public DeadRabbit(Image image, Rectangle drawArea)
			: base(image, drawArea, 5.0f) {}
	}
}