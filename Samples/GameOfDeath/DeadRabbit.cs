using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Sprites;

namespace GameOfDeath
{
	/// <summary>
	/// Dead rabbits are just simple fadeout effects and have no meaning to the game play.
	/// </summary>
	public class DeadRabbit : Sprite
	{
		public DeadRabbit(Image image, Rectangle drawArea)
			: base(image, drawArea)
		{
			Start<FinalTransition>().Add(new Transition.Duration(DeadRabbitDuration)).Add(
				new Transition.FadingColor(Color));
		}

		private const int DeadRabbitDuration = 5;
	}
}