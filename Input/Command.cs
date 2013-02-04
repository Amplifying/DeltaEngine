using System;
using System.Collections.Generic;
using DeltaEngine.Input.Triggers;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Provides the ability to attach triggers and callbacks to input events.
	/// </summary>
	public class Command
	{
		//TODO: 
		public Command()
		{
			Attach(trigger => TriggerFired = true);
		}

		protected internal readonly List<Trigger> attachedTriggers = new List<Trigger>();
		public event Action<Trigger> Callback;
		public bool TriggerFired { get; private set; }

		public Command Attach(Action<Trigger> action)
		{
			Callback += action;
			return this;
		}

		public void Add(Trigger trigger)
		{
			if (attachedTriggers.Contains(trigger) == false)
				attachedTriggers.Add(trigger);
		}

		public void Remove(Trigger trigger)
		{
			attachedTriggers.Remove(trigger);
		}

		internal void Run(Input input)
		{
			foreach (Trigger trigger in attachedTriggers)
				if (trigger.ConditionMatched(input))
					Invoke(trigger);
		}

		internal void Invoke(Trigger trigger)
		{
			Callback.Invoke(trigger);
		}
	}
}