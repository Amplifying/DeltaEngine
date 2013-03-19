using System;
using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Input.Devices;
using DeltaEngine.Input.Triggers;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Provides a way to check and trigger all input commands in the run loop.
	/// </summary>
	public class InputCommands : Runner
	{
		public InputCommands(Keyboard keyboard, Mouse mouse, Touch touch, GamePad gamePad)
		{
			this.keyboard = keyboard;
			this.mouse = mouse;
			this.touch = touch;
			this.gamePad = gamePad;
		}

		internal readonly Keyboard keyboard;
		internal readonly Mouse mouse;
		internal readonly Touch touch;
		internal readonly GamePad gamePad;

		public Command Add(Key key, Action callback)
		{
			return Add(key, State.Releasing, callback);
		}

		public Command Add(Key key, State keyState, Action callback)
		{
			var command = new Command();
			command.Callback += trigger => callback();
			command.Add(new KeyTrigger(key, keyState));
			Add(command);
			return command;
		}

		public Command Add(MouseButton mouseButton, Action<Mouse> callback)
		{
			return Add(mouseButton, State.Releasing, callback);
		}

		public Command Add(MouseButton mouseButton, State buttonState, Action<Mouse> callback)
		{
			var command = new Command();
			command.Callback += trigger => callback(mouse);
			command.Add(new MouseButtonTrigger(mouseButton, buttonState));
			Add(command);
			return command;
		}

		public Command AddMouseMovement(Action<Mouse> mouseMovementCallback)
		{
			var command = new Command();
			command.Callback += trigger => mouseMovementCallback(mouse);
			command.Add(new MouseMovementTrigger());
			Add(command);
			return command;
		}

		public Command Add(Action<Touch> touchHappenedCallback)
		{
			return Add(State.Releasing, touchHappenedCallback);
		}

		public Command Add(State touchState, Action<Touch> callback)
		{
			var command = new Command();
			command.Callback += trigger => callback(touch);
			command.Add(new TouchPressTrigger(touchState));
			Add(command);
			return command;
		}

		public Command Add(GamePadButton gamePadButton, State keyState, Action callback)
		{
			var command = new Command();
			command.Callback += trigger => callback();
			command.Add(new GamePadButtonTrigger(gamePadButton, keyState));
			Add(command);
			return command;
		}

		public void Add(Command command)
		{
			if (commands.Contains(command) == false)
				commands.Add(command);
		}

		private readonly List<Command> commands = new List<Command>();

		public void Remove(Command command)
		{
			commands.Remove(command);
		}

		public void Clear()
		{
			commands.Clear();
		}

		public int Count
		{
			get { return commands.Count; }
		}

		public void Run()
		{
			var activeCommands = new List<Command>(commands);
			foreach (Command command in activeCommands)
				command.Run(this);
		}
	}
}