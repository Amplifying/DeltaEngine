using DeltaEngine.Datatypes;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Tracks any mouse movement, useful to update cursor positions or check hover states.
	/// </summary>
	public class MouseMovementTrigger : Trigger
	{
		public override bool ConditionMatched(InputCommands input)
		{
			bool changedPosition = input.Mouse.IsAvailable &&
				input.Mouse.Position != lastPosition && lastPosition != Point.Unused;
			lastPosition = input.Mouse.Position;
			return changedPosition;
		}

		private Point lastPosition = Point.Unused;
	}
}