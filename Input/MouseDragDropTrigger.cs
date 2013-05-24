using DeltaEngine.Datatypes;

namespace DeltaEngine.Input
{
	internal class MouseDragDropTrigger : Trigger
	{
		public MouseDragDropTrigger(Rectangle startArea, MouseButton button)
		{
			this.startArea = startArea;
			this.button = button;
			StartDragPosition = Point.Unused;
		}

		private readonly Rectangle startArea;
		private readonly MouseButton button;

		public override bool ConditionMatched(InputCommands input)
		{
			if (startArea.Contains(input.Mouse.Position) &&
				input.Mouse.GetButtonState(button) == State.Pressing)
				StartDragPosition = input.Mouse.Position;
			else if (StartDragPosition != Point.Unused &&
				input.Mouse.GetButtonState(button) != State.Released)
			{
				if (StartDragPosition.DistanceTo(input.Mouse.Position) > 0.0025f)
					return true;
			}
			else
				StartDragPosition = Point.Unused;

			return false;
		}

		public Point StartDragPosition { get; private set; }
	}
}