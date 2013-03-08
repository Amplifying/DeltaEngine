using DeltaEngine.Datatypes;
using DeltaEngine.Input;

namespace Blocks
{
	/// <summary>
	/// Knits the main control classes together by feeding events raised by one to another
	/// </summary>
	public class Game
	{
		public Game(Controller controller, UserInterface userInterface, InputCommands input)
		{
			this.controller = controller;
			this.input = input;
			SetControllerEvents(userInterface);
			SetInputEvents();
		}

		private readonly Controller controller;
		private readonly InputCommands input;

		private void SetControllerEvents(UserInterface userInterface)
		{
			controller.AddToScore += userInterface.AddToScore;
			controller.Lose += userInterface.Lose;
		}

		private void SetInputEvents()
		{
			SetKeyboardEvents();
			SetMouseEvents();
			SetTouchEvents();
		}

		private void SetKeyboardEvents()
		{
			input.Add(Key.CursorLeft, State.Pressing, controller.MoveBlockLeftIfPossible);
			input.Add(Key.CursorRight, State.Pressing, controller.MoveBlockRightIfPossible);
			input.Add(Key.CursorUp, State.Pressing, controller.RotateBlockAntiClockwiseIfPossible);
			input.Add(Key.CursorDown, State.Pressing, () => { controller.IsFallingFast = true; });
			input.Add(Key.CursorDown, State.Releasing, () => { controller.IsFallingFast = false; });
		}

		private void SetMouseEvents()
		{
			input.Add(MouseButton.Left, State.Pressing, mouse => Pressing(mouse.Position));
			input.Add(MouseButton.Left, State.Releasing, mouse => { controller.IsFallingFast = false; });
		}

		private void Pressing(Point position)
		{
			if (position.X < 0.4f)
				controller.MoveBlockLeftIfPossible();
			else if (position.X > 0.6f)
				controller.MoveBlockRightIfPossible();
			else if (position.Y < 0.5f)
				controller.RotateBlockAntiClockwiseIfPossible();
			else
				controller.IsFallingFast = true;
		}

		private void SetTouchEvents()
		{
			input.Add(State.Pressing, touch => Pressing(touch.GetPosition(0)));
			input.Add(State.Releasing, touch => { controller.IsFallingFast = false; });
		}
	}
}