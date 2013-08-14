using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Drag events with Mouse.
	/// </summary>
	public class MouseDragTrigger : DrawAreaTrigger
	{
		public MouseDragTrigger(MouseButton button = MouseButton.Left)
			: base(Rectangle.Unused)
		{
			Button = button;
			Start<Mouse>();
		}

		public MouseButton Button { get; private set; }

		public MouseDragTrigger(string button)
			: base(Rectangle.Unused)
		{
			var parameters = button.SplitAndTrim(new[] { ' ' });
			Button = parameters.Length > 0 ? parameters[0].Convert<MouseButton>() : MouseButton.Left;
			Start<Mouse>();
		}
	}
}