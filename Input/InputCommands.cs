using System;
using System.Collections.Generic;
using DeltaEngine.Core;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Provides a way to check and trigger all input commands in the run loop.
	/// </summary>
	public class InputCommands : PriorityRunner
	{
		public InputCommands(Keyboard keyboard, PointerDevices pointerDevices, GamePad gamePad)
		{
			this.keyboard = keyboard;
			this.pointerDevices = pointerDevices;
			this.gamePad = gamePad;
		}

		internal readonly Keyboard keyboard;
		private readonly PointerDevices pointerDevices;
		internal readonly GamePad gamePad;

		internal Mouse Mouse
		{
			get { return pointerDevices.mouse; }
		}

		internal Touch Touch
		{
			get { return pointerDevices.touch; }
		}

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
			command.Callback += trigger => callback(Mouse);
			command.Add(new MouseButtonTrigger(mouseButton, buttonState));
			Add(command);
			return command;
		}

		public Command AddMouseMovement(Action<Mouse> mouseMovementCallback)
		{
			var command = new Command();
			command.Callback += trigger => mouseMovementCallback(Mouse);
			command.Add(new MouseMovementTrigger());
			Add(command);
			return command;
		}

		public Command AddMouseHover(Action<Mouse> mouseHoverCallback)
		{
			var command = new Command();
			command.Callback += trigger => mouseHoverCallback(Mouse);
			command.Add(new MouseHoverTrigger());
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
			command.Callback += trigger => callback(Touch);
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