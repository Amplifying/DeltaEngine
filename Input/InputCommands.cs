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

		public void Add(Key key, Action callback)
		{
			Add(key, State.Releasing, callback);
		}

		public void Add(Key key, State keyState, Action callback)
		{
			var command = new Command();
			command.Callback += trigger => callback();
			command.Add(new KeyTrigger(key, keyState));
			Add(command);
		}

		public void Add(MouseButton mouseButton, Action<Mouse> callback)
		{
			Add(mouseButton, State.Releasing, callback);
		}

		public void Add(MouseButton mouseButton, State buttonState, Action<Mouse> callback)
		{
			var command = new Command();
			command.Callback += trigger => callback(mouse);
			command.Add(new MouseButtonTrigger(mouseButton, buttonState));
			Add(command);
		}

		public void AddMouseMovement(Action<Mouse> mouseMovementCallback)
		{
			var command = new Command();
			command.Callback += trigger => mouseMovementCallback(mouse);
			command.Add(new MouseMovementTrigger());
			Add(command);
		}

		public void Add(Action<Touch> touchHappenedCallback)
		{
			Add(State.Releasing, touchHappenedCallback);
		}

		public void Add(State touchState, Action<Touch> callback)
		{
			var command = new Command();
			command.Callback += trigger => callback(touch);
			command.Add(new TouchPressTrigger(touchState));
			Add(command);
		}

		public void Add(GamePadButton gamePadButton, State keyState, Action callback)
		{
			var command = new Command();
			command.Callback += trigger => callback();
			command.Add(new GamePadButtonTrigger(gamePadButton, keyState));
			Add(command);
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

		public int Count
		{
			get { return commands.Count; }
		}

		public void Run()
		{
			foreach (Command command in commands)
				command.Run(this);
		}
	}
}