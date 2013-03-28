using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Fires once when the mouse has not moved for a prescribed period.
	/// Ideally used in tandem with MouseMovementTrigger to cancel the logic 
	/// raised on a hover.
	/// </summary>
	public class MouseHoverTrigger : Trigger
	{
		public override bool ConditionMatched(InputCommands inputCommands, Time time)
		{
			if (!inputCommands.Mouse.IsAvailable)
				return false;

			if (lastPosition == inputCommands.Mouse.Position)
				return ProcessHover(time);

			lastPosition = inputCommands.Mouse.Position;
			elapsed = 0.0f;
			return false;
		}

		private Point lastPosition;
		private float elapsed;

		private bool ProcessHover(Time time)
		{
			if (elapsed >= HoverTime)
				return false;

			elapsed += time.CurrentDelta;
			return elapsed >= HoverTime;
		}

		private const float HoverTime = 1.5f;
	}
}