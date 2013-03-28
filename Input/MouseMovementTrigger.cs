using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Tracks any mouse movement, useful to update cursor positions or check hover states.
	/// </summary>
	public class MouseMovementTrigger : Trigger
	{
		public override bool ConditionMatched(InputCommands inputCommands, Time time)
		{
			bool changedPosition = inputCommands.Mouse.IsAvailable &&
				inputCommands.Mouse.Position != lastPosition && lastPosition != UnusedPosition;
			lastPosition = inputCommands.Mouse.Position;
			return changedPosition;
		}

		private Point lastPosition = UnusedPosition;
		private static readonly Point UnusedPosition = new Point(-1, -1);
	}
}