using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Shapes;

namespace SideScrollerSample
{
	internal class InteractionLogics : Entity
	{
		public void FireShotByPlayer(Point startPosition)
		{
			var bulletTrail = new Line2D(startPosition, new Point(1, startPosition.Y), Color.Orange);
			bulletTrail.Add<Transition>().Add(new Transition.Duration(0.2f)).Add(
				new Transition.Color(Color.Orange, Color.TransparentBlack));
		}

		public void FireShotByEnemy(Point startPosition)
		{
			var bulletTrail = new Line2D(startPosition, new Point(0, startPosition.Y), Color.Red);
			bulletTrail.Add<Transition>().Add(new Transition.Duration(0.2f)).Add(
				new Transition.Color(Color.Red, Color.TransparentBlack));
		}
	}
}