using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Fires once when the mouse has not moved for a prescribed period. Ideally used in tandem with
	/// MouseMovementTrigger to cancel the logic raised on a hover.
	/// </summary>
	public class MouseHoverTrigger : Trigger
	{
		public MouseHoverTrigger(float hoverTime = DefaultHoverTime)
		{
			this.hoverTime = hoverTime;
		}

		private readonly float hoverTime;
		private const float DefaultHoverTime = 1.5f;

		public override bool ConditionMatched(InputCommands input)
		{
			if (!input.Mouse.IsAvailable)
				return false; //ncrunch: no coverage

			if (lastPosition.DistanceTo(input.Mouse.Position) < 0.0025f)
				return ProcessHover();

			lastPosition = input.Mouse.Position;
			elapsed = 0.0f;
			return false;
		}

		private Point lastPosition;
		private float elapsed;

		private bool ProcessHover()
		{
			if (elapsed >= hoverTime)
				return false;

			elapsed += Time.Current.Delta;
			return elapsed >= hoverTime;
		}
	}
}