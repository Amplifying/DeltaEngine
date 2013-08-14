using System;
using DeltaEngine.Commands;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Tracks any mouse movement, useful to update cursor positions or check hover states.
	/// </summary>
	public class MouseMovementTrigger : PositionTrigger
	{
		public MouseMovementTrigger()
			: base(Point.Unused)
		{
			Start<Mouse>();
		}

		public MouseMovementTrigger(string empty)
			: base(Point.Unused)
		{
			if (!String.IsNullOrEmpty(empty))
				throw new MouseMovementTriggerHasNoParameters();
			Start<Mouse>();
		}

		public class MouseMovementTriggerHasNoParameters : Exception {}
	}
}