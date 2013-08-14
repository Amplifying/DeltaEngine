using System;
using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;

namespace DeltaEngine.Commands
{
	/// <summary>
	/// Input Commands are loaded via the InputCommands.xml file, see InputCommands.cs for details.
	/// You can also create your own commands, which will be executed whenever any trigger is invoked.
	/// </summary>
	public class Command : Entity, Updateable
	{
		static Command()
		{
			RegisteredCommands.Use(new Dictionary<string, Trigger[]>());
		}

		private static readonly ThreadStatic<Dictionary<string, Trigger[]>> RegisteredCommands =
			new ThreadStatic<Dictionary<string, Trigger[]>>();

		public static void Register(string commandName, params Trigger[] commandTriggers)
		{
			if (string.IsNullOrEmpty(commandName))
				throw new ArgumentNullException("commandName");
			if (commandTriggers == null || commandTriggers.Length == 0)
				throw new UnableToRegisterCommandWithoutTriggers(commandName);
			if (RegisteredCommands.Current.ContainsKey(commandName))
				RegisteredCommands.Current[commandName] = commandTriggers;
			else
				RegisteredCommands.Current.Add(commandName, commandTriggers);
		}

		public class UnableToRegisterCommandWithoutTriggers : Exception
		{
			public UnableToRegisterCommandWithoutTriggers(string commandName)
				: base(commandName) {}
		}

		public Command(string commandName, Action action)
		{
			triggers.AddRange(LoadTriggersForCommand(commandName));
			this.action = action;
			UpdatePriority = Priority.First;
		}

		private readonly Action action;

		private static IEnumerable<Trigger> LoadTriggersForCommand(string commandName)
		{
			Trigger[] loadedTriggers;
			if (RegisteredCommands.Current.TryGetValue(commandName, out loadedTriggers))
				return loadedTriggers;
			throw new CommandNameWasNotRegistered();
		}

		public class CommandNameWasNotRegistered : Exception {}

		public Command(string commandName, Action<Point> positionAction)
		{
			triggers.AddRange(LoadTriggersForCommand(commandName));
			this.positionAction = positionAction;
			UpdatePriority = Priority.First;
		}

		public Command(string commandName, Action<Rectangle> drawAreaAction)
		{
			triggers.AddRange(LoadTriggersForCommand(commandName));
			this.drawAreaAction = drawAreaAction;
			UpdatePriority = Priority.First;
		}

		public Command(Action action)
		{
			this.action = action;
			UpdatePriority = Priority.First;
		}

		public Command(Action<Point> positionAction)
		{
			this.positionAction = positionAction;
			UpdatePriority = Priority.First;
		}

		private readonly Action<Point> positionAction;

		public Command(Action<Rectangle> drawAreaAction)
		{
			this.drawAreaAction = drawAreaAction;
			UpdatePriority = Priority.First;
		}

		private readonly Action<Rectangle> drawAreaAction;

		public Command Add(Trigger trigger)
		{
			triggers.Add(trigger);
			return this;
		}

		private readonly List<Trigger> triggers = new List<Trigger>();

		public void Update()
		{
			Trigger invokedTrigger = triggers.Find(t => t.WasInvoked);
			if (invokedTrigger == null)
				return;
			var positionTrigger = invokedTrigger as PositionTrigger;
			var drawAreaTrigger = invokedTrigger as DrawAreaTrigger;
			if (positionAction != null && positionTrigger != null)
				positionAction(positionTrigger.Position);
			else if (drawAreaAction != null && drawAreaTrigger != null)
				drawAreaAction(drawAreaTrigger.DrawArea);
			else if (action != null)
				action();
			else if (positionAction != null)
				positionAction(Point.Half);
			else if (drawAreaAction != null)
				drawAreaAction(Rectangle.One);
		}

		public List<Trigger> GetTriggers()
		{
			return triggers;
		}
	}
}