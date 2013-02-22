using DeltaEngine.Datatypes;

namespace DeltaEngine.Input.Triggers
{
	/// <summary>
	/// Tracks any mouse movement, useful to update cursor positions or check hover states.
	/// </summary>
	public class MouseMovementTrigger : Trigger
	{
		public override bool ConditionMatched(InputCommands inputCommands)
		{
			bool changedPosition = inputCommands.mouse.IsAvailable &&
				inputCommands.mouse.Position != lastPosition && lastPosition != UnusedPosition;
			lastPosition = inputCommands.mouse.Position;
			return changedPosition;
		}

		private Point lastPosition = UnusedPosition;
		private static readonly Point UnusedPosition = new Point(-1, -1);
	}
}