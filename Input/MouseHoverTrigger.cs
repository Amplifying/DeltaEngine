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
		public override bool ConditionMatched(InputCommands inputCommands)
		{
			if (!inputCommands.Mouse.IsAvailable)
				return false; //ncrunch: no coverage

			if (lastPosition == inputCommands.Mouse.Position)
				return ProcessHover();

			lastPosition = inputCommands.Mouse.Position;
			elapsed = 0.0f;
			return false;
		}

		private Point lastPosition;
		private float elapsed;

		private bool ProcessHover()
		{
			if (elapsed >= HoverTime)
				return false;

			elapsed += Time.Current.Delta;
			return elapsed >= HoverTime;
		}

		private const float HoverTime = 1.5f;
	}
}