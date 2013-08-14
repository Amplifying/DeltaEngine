using System;
using DeltaEngine.Commands;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Tracks any touch movement, useful to update cursor positions.
	/// </summary>
	public class TouchMovementTrigger : PositionTrigger
	{
		public TouchMovementTrigger()
			: base(Point.Unused)
		{
			Start<Touch>();
		}

		public TouchMovementTrigger(string empty)
			: base(Point.Unused)
		{
			if (!String.IsNullOrEmpty(empty))
				throw new TouchMovementTriggerHasNoParameters();
			Start<Touch>();
		}

		public class TouchMovementTriggerHasNoParameters : Exception {}
	}
}