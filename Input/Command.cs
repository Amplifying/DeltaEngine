using System;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Provides the ability to attach triggers and callbacks to input events.
	/// </summary>
	public sealed class Command
	{
		public Command()
		{
			Attach(trigger => TriggerFired = true);
		}

		private readonly List<Trigger> attachedTriggers = new List<Trigger>();

		public bool TriggerFired { get; private set; }

		public void Attach(Action<Trigger> action)
		{
			Callback += action;
		}

		public event Action<Trigger> Callback;

		public int NumberOfAttachedTriggers
		{
			get { return attachedTriggers.Count; }
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

		internal void Run(InputCommands inputCommands, Time time)
		{
			var copy = new List<Trigger>(attachedTriggers);
			foreach (
				Trigger trigger in copy.Where(trigger => trigger.ConditionMatched(inputCommands, time)))
				Invoke(trigger);
		}

		private void Invoke(Trigger trigger)
		{
			Callback.Invoke(trigger);
		}
	}
}