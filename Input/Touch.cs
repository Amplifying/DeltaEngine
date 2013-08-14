using System;
using System.Collections.Generic;
using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Provides a way to fetch the current input values from a Touch device.
	/// </summary>
	public abstract class Touch : InputDevice
	{
		public abstract Point GetPosition(int touchIndex);
		public abstract State GetState(int touchIndex);

		public override void Update(IEnumerable<Entity> entities)
		{
			if (!IsAvailable)
				return; //ncrunch: no coverage
			foreach (Entity entity in entities)
			{
				TryInvokeTriggerOfType<TouchPressTrigger>(entity, IsTouchPressTriggered);
				TryInvokeTriggerOfType<TouchMovementTrigger>(entity, IsTouchMovementTriggered);
				TryInvokeTriggerOfType<TouchPositionTrigger>(entity, IsTouchPositionTriggered);
			}
		}

		private static void TryInvokeTriggerOfType<T>(Entity entity, Func<T, bool> triggeredCode)
			where T : Trigger
		{
			var trigger = entity as T;
			if (trigger != null)
				trigger.WasInvoked = triggeredCode.Invoke(trigger);
		}

		private bool IsTouchPressTriggered(TouchPressTrigger trigger)
		{
			return GetState(0) == trigger.State;
		}

		private bool IsTouchMovementTriggered(TouchMovementTrigger trigger)
		{
			bool changedPosition = trigger.Position != GetPosition(0) &&
				trigger.Position != Point.Unused;
			trigger.Position = GetPosition(0);
			return changedPosition;
		}

		private bool IsTouchPositionTriggered(TouchPositionTrigger trigger)
		{
			var isButton = GetState(0) == trigger.State;
			bool hasPositionChanged = trigger.Position != GetPosition(0) &&
				trigger.Position != Point.Unused;
			trigger.Position = GetPosition(0);
			return isButton && hasPositionChanged;
		}
	}
}