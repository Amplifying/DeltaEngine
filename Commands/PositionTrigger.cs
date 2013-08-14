using DeltaEngine.Datatypes;

namespace DeltaEngine.Commands
{
	/// <summary>
	/// Allows a trigger to be invoked along with any associated Command or Entity.
	/// </summary>
	public abstract class PositionTrigger : Trigger
	{
		protected PositionTrigger(Point position)
		{
			Position = position;
		}

		public Point Position { get; set; }
	}
}