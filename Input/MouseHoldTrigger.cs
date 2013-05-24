using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Fires once when the mouse button is pressed and the mouse has not moved for some time.
	/// </summary>
	public class MouseHoldTrigger : Trigger
	{
		public MouseHoldTrigger(Rectangle holdArea, float holdTime = DefaultHoldTime,
			MouseButton button = MouseButton.Left)
		{
			this.holdArea = holdArea;
			this.holdTime = holdTime;
			this.button = button;
		}

		private readonly Rectangle holdArea;
		private readonly float holdTime;
		private readonly MouseButton button;
		private const float DefaultHoldTime = 1.0f;

		public override bool ConditionMatched(InputCommands input)
		{
			if (!input.Mouse.IsAvailable)
				return false; //ncrunch: no coverage

			if (input.Mouse.GetButtonState(button) == State.Pressing)
				startPosition = input.Mouse.Position;

			if (holdArea.Contains(startPosition) &&
				input.Mouse.GetButtonState(button) == State.Pressed &&
				lastPosition.DistanceTo(input.Mouse.Position) < 0.0025f)
				return ProcessHover();

			lastPosition = input.Mouse.Position;
			elapsed = 0.0f;
			return false;
		}

		private Point startPosition;
		private Point lastPosition;
		private float elapsed;

		private bool ProcessHover()
		{
			if (elapsed >= holdTime)
				return false;

			elapsed += Time.Current.Delta;
			return elapsed >= holdTime;
		}
	}
}